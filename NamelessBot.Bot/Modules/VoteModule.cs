using Kook.Commands;
using NamelessBot.Bot.Models.Votes;
using NamelessBot.Bot.Services;

namespace NamelessBot.Bot.Modules {
    //[Summary("投票模块")]
    //[Group("vote")]
    //public class VoteModule : ModuleBase<SocketCommandContext> {
    //    private readonly VoteService _voteService;

    //    public VoteModule(VoteService voteService) {
    //        _voteService = voteService;
    //    }

    //    [Summary("创建投票")]
    //    [Command("create")]
    //    [RequireContext(ContextType.Guild)]
    //    public async Task CreateVote(string title, TimeSpan endTime, bool withDescription, params string[] items) {
    //        List<VoteItem> voteItems = new List<VoteItem>();
    //        if (withDescription) {
    //            for (int i = 0; i != items.Length; i = i + 2) {
    //                voteItems.Add(new VoteItem(Context.Client) { Title = items[i], Description = items[i + 1] });
    //            }

    //        } else {
    //            foreach (var item in items) {
    //                voteItems.Add(new VoteItem(Context.Client) { Title = item, Description = null });
    //            }
    //        }

    //        await _voteService.CreateVoteAsync(Context.Channel.Id, DateTimeOffset.Now.Add(endTime), title, voteItems.ToArray());
    //    }

    //    [Summary("查看结果")]
    //    [Command("result")]
    //    [RequireContext(ContextType.Guild)]
    //    public async Task ShowResult(string Id) {
    //        await _voteService.ShowResult(Guid.Parse(Id), Context.Channel.Id);
    //    }

    //    [Summary("结束投票")]
    //    [Command("end")]
    //    [RequireContext(ContextType.Guild)]
    //    public async Task EndVote(string Id) {
    //        await _voteService.EndVote(Guid.Parse(Id));
    //    }
    //}
}
