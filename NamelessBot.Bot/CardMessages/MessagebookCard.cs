using Kook;
using NamelessBot.Bot.Models.Messagebook;

namespace NamelessBot.Bot.CardMessages {
    public class MessagebookCard {
        public Messagebook Book;
        public ulong BotId;

        public MessagebookCard(Messagebook book, ulong botId) {
            Book = book;
            BotId = botId;
        }

        public Card[] Build() {
            string messageText = "";
            for (int i = Book.Messages.Count - 1; i != -1; i--) {
                if (Book.Messages[i].IsReview) {
                    if (i - 1 != -1) {
                        messageText += $"> {Book.Messages[i].Message}\n\n---\n";
                    } else {
                        messageText += $"> {Book.Messages[i].Message}";
                    }
                }
            }

            var messagesCard = new CardBuilder()
                .WithSize(CardSize.Large).WithTheme(CardTheme.Info)
                .AddModule(new HeaderModuleBuilder().WithText(new PlainTextElementBuilder().WithContent("留言")))
                .AddModule(new SectionModuleBuilder().WithText(new KMarkdownElementBuilder().WithContent(messageText)));
            var infoCard = new CardBuilder()
                .WithSize(CardSize.Large).WithTheme(CardTheme.Info)
                .AddModule(new HeaderModuleBuilder().WithText(new PlainTextElementBuilder().WithContent(Book.Title)))
                .AddModule(new SectionModuleBuilder().WithText(new KMarkdownElementBuilder().WithContent(Book.Description)))
                .AddModule(new ContextModuleBuilder().AddElement(new KMarkdownElementBuilder().WithContent($"私聊机器人 (met){BotId}(met) 以下命令留言")));

            return new Card[] {
                infoCard.Build(),
                messagesCard.Build()
            };
        }
    }
}
