using KaiHeiLa.WebSocket;
using NamelessBot.Bot.Models.Enity;

namespace NamelessBot.Bot.Models.Messagebook {
    public class MessagebookMessage : EnityBase {
        public string Message { get; set; }
        public bool IsReview { get; set; } = false;
    }
}
