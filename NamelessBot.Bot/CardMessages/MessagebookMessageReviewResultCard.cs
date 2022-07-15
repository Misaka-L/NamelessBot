using KaiHeiLa;
using NamelessBot.Bot.Models.ButtonValue;
using NamelessBot.Bot.Models.Messagebook;
using Newtonsoft.Json;

namespace NamelessBot.Bot.CardMessages {
    public class MessagebookMessageReviewResultCard {
        public Messagebook Messagebook;
        public MessagebookMessage MessagebookMessage;

        public MessagebookMessageReviewResultCard(Messagebook messagebook, MessagebookMessage message) {
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
                .AddElement(new ButtonElementBuilder().WithText(new KMarkdownElementBuilder().WithContent("删除留言")).WithClick(ButtonClickEventType.ReturnValue).WithTheme(ButtonTheme.Danger).WithValue(JsonConvert.SerializeObject(new ReviewAction {
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
