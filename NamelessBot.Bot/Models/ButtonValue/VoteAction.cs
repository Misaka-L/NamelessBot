namespace NamelessBot.Bot.Models.ButtonValue {
    public class VoteAction : IButtonValue {
        public string Type { get; } = "vote";
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
    }
}
