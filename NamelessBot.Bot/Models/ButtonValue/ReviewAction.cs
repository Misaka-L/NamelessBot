namespace NamelessBot.Bot.Models.ButtonValue {
    public class ReviewAction : IButtonValue {
        public string Type { get; } = "messagebook_review";
        public Guid Id { get; set; }
        public Guid MessageId { get; set; }
        public bool Pass { get; set; }
    }
}
