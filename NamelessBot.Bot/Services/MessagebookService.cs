using Kook;
using Kook.WebSocket;
using Microsoft.Extensions.Logging;
using NamelessBot.Bot.CardMessages;
using NamelessBot.Bot.Models.ButtonValue;
using NamelessBot.Bot.Models.Messagebook;
using Newtonsoft.Json;

namespace NamelessBot.Bot.Services
{
    public class MessagebookService
    {
        private readonly ILogger<MessagebookService> _logger;
        private readonly KookSocketClient _socketClient;

        public IReadOnlyCollection<Messagebook> Messagebooks => _messagebooks.AsReadOnly();
        private List<Messagebook> _messagebooks = new List<Messagebook>();
        private Dictionary<ulong, uint[]> _guildManagerRole = new Dictionary<ulong, uint[]>();

        public MessagebookService(ILogger<MessagebookService> logger, KookSocketClient socketClient)
        {
            _logger = logger;
            _socketClient = socketClient;

            _socketClient.MessageButtonClicked += _socketClient_MessageButtonClicked;
            LoadMessagebooks();
        }

        private async Task _socketClient_MessageButtonClicked(string value, Cacheable<SocketGuildUser, ulong> cacheable,
            Cacheable<IMessage, Guid> arg3, SocketTextChannel channel)
        {
            var data = JsonConvert.DeserializeObject<ReviewAction>(value);
            if (data.Type == "messagebook_review")
            {
                await ReviewMessage(data.Id, data.MessageId, data.Pass, arg3.Id);
            }
        }

        #region S/L Messagebooks

        public void SaveMessagebooks()
        {
            File.WriteAllText("messagebooks.json", JsonConvert.SerializeObject(new MessagebookData
            {
                GuildManagerRole = _guildManagerRole,
                Messagebooks = _messagebooks
            }, Formatting.Indented));
        }

        public void LoadMessagebooks()
        {
            if (File.Exists("messagebooks.json"))
            {
                var data = JsonConvert.DeserializeObject<MessagebookData>(File.ReadAllText("messagebooks.json"));
                _guildManagerRole = data.GuildManagerRole;
                _messagebooks = data.Messagebooks;
            }
            else
            {
                SaveMessagebooks();
            }
        }

        #endregion

        #region Messagebook Manager

        public async Task CreateMessagebook(string title, string description, ulong channelId)
        {
            var channel = await _socketClient.GetChannelAsync(channelId) as SocketTextChannel;
            var book = new Messagebook
                { Title = title, Description = description, ChannelId = channelId, GuildId = channel.Guild.Id };
            var cards = new MessagebookCard(book, _socketClient.CurrentUser.Id).Build();
            await channel.SendCardAsync(cards[0]);
            await channel.SendTextAsync($"`.mb message {book.Id} 留言内容`");
            book.MessageId = (await channel.SendCardAsync(cards[1])).Id;

            _messagebooks.Add(book);
            SaveMessagebooks();
            _logger.LogInformation("创建了留言板 {0}({1}):{2}", book.Title, book.Id, book.Description);
        }

        public async Task RemoveMessagebook(Guid id)
        {
            if (_messagebooks.Find(m => m.Id == id) is Messagebook messagebook)
            {
                _messagebooks.Remove(messagebook);
                if (!Directory.Exists("messagebooks")) Directory.CreateDirectory("messagebooks");
                await File.WriteAllTextAsync(
                    $"messagebooks/{messagebook.Title}-{DateTimeOffset.Now.ToString("yyyy-MM-dd-HH-mm-ss-ms")}.json",
                    JsonConvert.SerializeObject(messagebook, Formatting.Indented));

                SaveMessagebooks();
                await (await _socketClient.GetChannelAsync(messagebook.ChannelId) as SocketTextChannel).SendTextAsync(
                    "你现在可以安全地删除留言板消息了");
                _logger.LogInformation("已删除留言板 {0}({1})", messagebook.Title, messagebook.Id);
            }
        }

        public async Task UpdateMessagebook(Guid id)
        {
            if (_messagebooks.Find(book => book.Id == id) is Messagebook messagebook)
            {
                var cards = new MessagebookCard(messagebook, _socketClient.CurrentUser.Id).Build().ToList();
                cards.RemoveAt(0);

                await (await _socketClient.GetChannelAsync(messagebook.ChannelId) as SocketTextChannel)
                    .ModifyMessageAsync(messagebook.MessageId, (properties) => { properties.Cards = cards; });

                _logger.LogInformation("已更新留言板 {0}({1})", messagebook.Title, messagebook.Id);
            }
        }

        #endregion

        #region Message & Review

        public async Task CreateMessage(Guid id, string message, IUser user)
        {
            if (_messagebooks.Find(book => book.Id == id) is Messagebook messagebook)
            {
                var messagebookMessage = new MessagebookMessage { Message = message, CreatorId = user.Id };
                messagebook.Messages.Add(messagebookMessage);
                SaveMessagebooks();
                _logger.LogInformation("已在留言板 {0}({1}) 创建 {2}({3}) 的留言: ({4}){5}", messagebook.Title, messagebook.Id,
                    $"{user.Username}#{user.IdentifyNumber}", user.Id, messagebookMessage.Id,
                    messagebookMessage.Message);

                await SendReviewMessage(messagebook, messagebookMessage);
            }
            else
            {
                throw new Exception("不存在该留言板");
            }
        }

        public async Task SendReviewMessage(Messagebook messagebook, MessagebookMessage message)
        {
            if (messagebook.ReviewChannelId.HasValue && !message.IsReview)
            {
                _logger.LogInformation("发送审核消息留言板 {0}({1}) {2} 的留言: ({4}){5}", messagebook.Title, messagebook.Id,
                    message.CreatorId, message.Id, message.Message);
                await (await _socketClient.GetChannelAsync(messagebook.ReviewChannelId.GetValueOrDefault()) as
                    SocketTextChannel).SendCardsAsync(new MessagebookMessageReviewCard(messagebook, message).Build());
            }
        }

        public async Task ReviewMessage(Guid id, Guid messagebookMessageId, bool pass, Guid reviewMessageId)
        {
            if (_messagebooks.Find(book => book.Id == id) is Messagebook messagebook)
            {
                if (messagebook.Messages.Find(m => m.Id == messagebookMessageId) is MessagebookMessage message)
                {
                    var ownerUser = await message.GetCreatorAsync(_socketClient);
                    if (pass)
                    {
                        message.IsReview = true;
                        await (await _socketClient.GetChannelAsync(messagebook.ReviewChannelId.GetValueOrDefault()) as
                            SocketTextChannel).ModifyMessageAsync(reviewMessageId,
                            (properties) =>
                            {
                                properties.Cards = new MessagebookMessageReviewResultCard(messagebook, message).Build()
                                    .ToList();
                            });

                        _logger.LogInformation("已通过 {0}({1}) 的在留言板 {2}({3}) 的消息: ({4})({5})",
                            $"{ownerUser.Username}#{ownerUser.IdentifyNumber}", message.CreatorId, messagebook.Title,
                            messagebook.Id, message.Id, message.Message);
                    }
                    else
                    {
                        messagebook.Messages.Remove(message);
                        await (await _socketClient.GetChannelAsync(messagebook.ReviewChannelId.GetValueOrDefault()) as
                            SocketTextChannel).DeleteMessageAsync(reviewMessageId);
                        _logger.LogInformation("审核未通过，删除 {0}({1}) 的在留言板 {2}({3}) 的消息: ({4})({5})",
                            $"{ownerUser.Username}#{ownerUser.IdentifyNumber}", message.CreatorId, messagebook.Title,
                            messagebook.Id, message.Id, message.Message);
                    }

                    SaveMessagebooks();
                    await UpdateMessagebook(id);
                }
            }
        }

        public async Task SetReviewChannel(Guid id, SocketTextChannel channel)
        {
            if (_messagebooks.Find(book => book.Id == id) is Messagebook messagebook)
            {
                messagebook.ReviewChannelId = channel.Id;
                SaveMessagebooks();
                _logger.LogInformation("频道 {0}({1}) 已被设为 {2}({3}) 的审核频道", channel.Name, channel.Id, messagebook.Title,
                    messagebook.Id);
                await channel.SendTextAsync($"该频道已被设为 {messagebook.Title} ({messagebook.Id}) 的审核频道");

                foreach (var message in messagebook.Messages)
                {
                    await SendReviewMessage(messagebook, message);
                }
            }
        }

        #endregion

        #region Permission

        public async Task<bool> CheckPermission(ulong guildId, IUser user)
        {
            if (!_guildManagerRole.ContainsKey(guildId)) return false;
            if (_guildManagerRole[guildId] is uint[] roles)
            {
                var userRoles = (await _socketClient.Rest.GetGuildUserAsync(guildId, user.Id)).RoleIds;
                foreach (var role in userRoles)
                {
                    if (roles.Contains(role)) return true;
                }
            }

            return false;
        }

        public void AddManagerRole(ulong guildId, uint role)
        {
            List<uint> roles = new List<uint>();
            if (_guildManagerRole.ContainsKey(guildId))
            {
                var originRoles = _guildManagerRole[guildId];
                roles = originRoles.ToList();
                if (!roles.Contains(role)) roles.Add(role);

                _guildManagerRole[guildId] = roles.ToArray();
                SaveMessagebooks();
                return;
            }

            roles.Add(role);
            _guildManagerRole.Add(guildId, roles.ToArray());
            SaveMessagebooks();
        }

        public void RemoveManagerRole(ulong guildId, uint role)
        {
            if (_guildManagerRole.ContainsKey(guildId))
            {
                var originRoles = _guildManagerRole[guildId];
                var roles = originRoles.ToList();
                if (roles.Contains(role)) roles.Remove(role);

                _guildManagerRole[guildId] = roles.ToArray();
            }

            SaveMessagebooks();
        }

        #endregion
    }
}