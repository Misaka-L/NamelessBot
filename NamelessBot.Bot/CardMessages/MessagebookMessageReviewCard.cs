using Kook;
using NamelessBot.Bot.Models.ButtonValue;
using NamelessBot.Bot.Models.Messagebook;
using Newtonsoft.Json;

namespace NamelessBot.Bot.CardMessages {
    public class MessagebookMessageReviewCard {
        public Messagebook Messagebook;
        public MessagebookMessage MessagebookMessage;

        public MessagebookMessageReviewCard(Messagebook messagebook, MessagebookMessage message) {
            Messagebook = messagebook;
            MessagebookMessage = message;
        }

        public Card[] Build() {
            var card = new CardBuilder().WithTheme(CardTheme.Primary).WithSize(CardSize.Large)
                .AddModule(new HeaderModuleBuilder().WithText(new PlainTextElementBuilder().WithContent(Messagebook.Title)))
                .AddModule(new SectionModuleBuilder().WithText(new KMarkdownElementBuilder().WithContent(Messagebook.Description)))
                .AddModule(new DividerModuleBuilder())
                .AddModule(new SectionModuleBuilder().WithText(new KMarkdownElementBuilder().WithContent($"> {MessagebookMessage.Message}")))
                .AddModule(new ActionGroupModuleBuilder()
                .AddElement(new ButtonElementBuilder().WithText(new KMarkdownElementBuilder().WithContent("通过")).WithClick(ButtonClickEventType.ReturnValue).WithTheme(ButtonTheme.Primary).WithValue(JsonConvert.SerializeObject(new ReviewAction {
                    Id = Messagebook.Id,
                    MessageId = MessagebookMessage.Id,
                    Pass = true
                })))
                .AddElement(new ButtonElementBuilder().WithText(new KMarkdownElementBuilder().WithContent("不通过")).WithClick(ButtonClickEventType.ReturnValue).WithTheme(ButtonTheme.Danger).WithValue(JsonConvert.SerializeObject(new ReviewAction {
                    Id = Messagebook.Id,
                    MessageId = MessagebookMessage.Id,
                    Pass = false
                }))))
                .Build();

            return new Card[] {
                card
            };
        }
    }
}
