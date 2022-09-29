using Kook.WebSocket;
using NamelessBot.Bot.Models.Enity;
using NamelessBot.Bot.Models.Votes;

namespace NamelessBot.Bot.Models {
    public class Vote : EnityBase {
        public Vote(KookSocketClient socketClient) { }

        public string Title { get; set; }
        public VoteItem[] Items { get; set; }
        public List<VoteUser> Users { get; set; } = new List<VoteUser>();
        public DateTimeOffset EndTime { get; set; }
        public Guid MessageId { get; set; }
    }
}
