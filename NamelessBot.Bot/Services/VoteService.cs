using KaiHeiLa;
using KaiHeiLa.WebSocket;
using Microsoft.Extensions.Logging;
using NamelessBot.Bot.CardMessages;
using NamelessBot.Bot.Models;
using NamelessBot.Bot.Models.ButtonValue;
using NamelessBot.Bot.Models.Votes;
using Newtonsoft.Json;

namespace NamelessBot.Bot.Services {
    public class VoteService {
        public IReadOnlyCollection<Vote> Votes => _votes.AsReadOnly();
        private List<Vote> _votes = new List<Vote>();

        private readonly KaiHeiLaSocketClient _socketClient;
        private readonly ILogger<VoteService> _logger;

        public VoteService(KaiHeiLaSocketClient socketCLient, ILogger<VoteService> logger) {
            _socketClient = socketCLient;
            _socketClient.MessageButtonClicked += _socketClient_MessageButtonClicked;
            _logger = logger;

            LoadConfig();
        }

        public void LoadConfig() {
            if (File.Exists("votes.json")) {
                try {
                    _votes = JsonConvert.DeserializeObject<List<Vote>>(File.ReadAllText("votes.json"));
                } catch (Exception ex) {
                    _logger.LogError(ex, "无法加载投票文件");
                    throw;
                }
            } else {
                SaveConfig();
            }
        }

        public void SaveConfig() {
            try {
                File.WriteAllText("votes.json", JsonConvert.SerializeObject(_votes, Formatting.Indented, new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }));
            } catch (Exception ex) {
                _logger.LogError(ex, "SaveConfig Error");
            }
        }

        private Task _socketClient_MessageButtonClicked(string content, SocketUser user, KaiHeiLa.IMessage sourceMessage, SocketTextChannel channel, SocketGuild guild) {
            var action = JsonConvert.DeserializeObject<VoteAction>(content);
            Vote(action.Id, user, action.ItemId, channel);

            return Task.CompletedTask;
        }

        public async Task CreateVoteAsync(ulong channelId, DateTimeOffset endTime, string title, VoteItem[] items) {
            var vote = new Vote(_socketClient) { Title = title, Items = items, ChannelId = channelId, EndTime = endTime };

            var channel = await _socketClient.GetChannelAsync(channelId) as SocketTextChannel;
            var result = await channel.SendCardMessageAsync(new VoteCard(vote).Build());

            vote.MessageId = result.MessageId;
            _votes.Add(vote);
            SaveConfig();
        }

        public async Task Vote(Guid Id, IUser user, Guid itemId, SocketTextChannel channel) {
            if (_votes.Find(v => v.Id == Id) is Vote vote) {
                if (vote.Users.Any(u => u.CreatorId == user.Id)) {
                    var item = vote.Items.First(item => item.Id == itemId);
                    var selectedItem = vote.Items.First(item => item.Id == vote.Users.Find(u => u.CreatorId == user.Id).ItemId);
                    _logger.LogInformation("{0}({1}) 给 {2}({3}) 的选项 {4}({5}) 尝试投票，但是忘记了他投过 {6}({7}) 了", $"{user.Username}#{user.IdentifyNumber}", user.Id, vote.Title, Id, item.Title, item.Id, selectedItem.Title, selectedItem.Id);

                    await channel.SendKMarkdownMessageAsync($"你已经投过 {selectedItem.Title} 了", ephemeralUser: user);
                } else {
                    vote.Users.Add(new VoteUser(_socketClient) { CreatorId = user.Id, ItemId = itemId });
                    var item = vote.Items.First(item => item.Id == itemId);
                    item.Count++;
                    SaveConfig();

                    await channel.SendKMarkdownMessageAsync($"你给 {item.Title} 投了一票", ephemeralUser: user);
                    _logger.LogInformation("{0}({1}) 给 {2}({3}) 的选项 {4}({5}) 投了一票，现在票数: {6}", $"{user.Username}#{user.IdentifyNumber}", user.Id, vote.Title, Id, item.Title, item.Id, item.Count);
                }
            } else {
                // todo
            }
        }

        public async Task EndVote(Guid Id) {
            if (_votes.Find(v => v.Id == Id) is Vote vote) {
                _logger.LogInformation("{0}({1}) 的投票结束了", vote.Title, vote.MessageId);
                _votes.Remove(vote);

                if (!Directory.Exists("votes")) Directory.CreateDirectory("votes");
                await File.WriteAllTextAsync($"votes/{vote.Title}-{DateTimeOffset.Now.ToString("yyyy-MM-dd-HH-mm-ss-ms")}.json", JsonConvert.SerializeObject(vote, Formatting.Indented));

                SaveConfig();
                await (await _socketClient.GetChannelAsync(vote.ChannelId) as SocketTextChannel).ModifyMessageAsync(vote.MessageId, (properties) => {
                    properties.Cards = new VoteResultCard(vote).Build().ToList();
                });
            }
        }

        public async Task ShowResult(Guid Id, ulong channelId) {
            if (_votes.Find(v => v.Id == Id) is Vote vote) {
                await (await _socketClient.GetChannelAsync(channelId) as SocketTextChannel).SendCardMessageAsync(new VoteResultCard(vote).Build());
            }
        }
    }
}
