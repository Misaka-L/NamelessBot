using KaiHeiLa;
using KaiHeiLa.Commands;
using KaiHeiLa.WebSocket;
using NamelessBot.Bot.Services;

namespace NamelessBot.Bot.Modules {
    [Summary("留言板")]
    [Group("mb")]
    public class MessagebookModule : ModuleBase<SocketCommandContext> {
        private readonly MessagebookService _messagebookService;

        public MessagebookModule(MessagebookService messagebookService) {
            _messagebookService = messagebookService;
        }

        [Summary("设置管理员角色组")]
        [Command("addManager")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task SetManager(IRole role) {
            _messagebookService.AddManagerRole(Context.Guild.Id, role.Id);
            await ReplyKMarkdownAsync($"已设置 (rol){role.Id}(rol) 为管理角色组");
        }

        [Summary("移除管理员角色组")]
        [Command("removeManager")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task RemoveManager(IRole role) {
            _messagebookService.RemoveManagerRole(Context.Guild.Id, role.Id);
            await ReplyKMarkdownAsync($"已移除 (rol){role.Id}(rol) 的管理权限");
        }

        [Summary("创建一个留言板")]
        [Command("create")]
        [RequireContext(ContextType.Guild)]
        public async Task CreateBook(string title, [Remainder] string description) {
            if (await _messagebookService.CheckPermission(Context.Guild.Id, Context.User)) {
                await _messagebookService.CreateMessagebook(title, description, Context.Channel.Id);
            }
        }

        [Summary("留言")]
        [Command("message")]
        [RequireContext(ContextType.DM)]
        public async Task CreateMessage(string id, [Remainder] string message) {
            try {
                await _messagebookService.CreateMessage(Guid.Parse(id), message, Context.User);
                await ReplyKMarkdownAsync($"留言成功，请等待管理员审核");
            } catch (Exception ex) {
                await ReplyKMarkdownAsync($"留言失败: {ex.Message}");
            }
        }

        [Summary("设置审核频道")]
        [Command("review")]
        [RequireContext(ContextType.Guild)]
        public async Task SetReviewChannel(string id) {
            if (await _messagebookService.CheckPermission(Context.Guild.Id, Context.User)) {
                await _messagebookService.SetReviewChannel(Guid.Parse(id), Context.Channel as SocketTextChannel);
            }
        }

        [Summary("删除留言板")]
        [Command("remove")]
        [RequireContext(ContextType.Guild)]
        public async Task RemoveMessage(string id) {
            if (await _messagebookService.CheckPermission(Context.Guild.Id, Context.User)) {
                await _messagebookService.RemoveMessagebook(Guid.Parse(id));
            }
        }

        [Summary("更新留言板")]
        [Command("update")]
        [RequireContext(ContextType.Guild)]
        public async Task UpdateMessagebook(string id) {
            if (await _messagebookService.CheckPermission(Context.Guild.Id, Context.User)) {
                await _messagebookService.UpdateMessagebook(Guid.Parse(id));
            }
        }
    }
}
