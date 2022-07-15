using KaiHeiLa.WebSocket;
using NamelessBot.Bot.Models.Enity;

namespace NamelessBot.Bot.Models.Messagebook {
    public class Messagebook : EnityBase {
        public Guid MessageId { get; set; }
        public ulong? ReviewChannelId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<MessagebookMessage> Messages { get; set; } = new List<MessagebookMessage>();
    }
}
