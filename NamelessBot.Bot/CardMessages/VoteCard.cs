using Kook;
using NamelessBot.Bot.Models;
using NamelessBot.Bot.Models.ButtonValue;
using Newtonsoft.Json;

namespace NamelessBot.Bot.CardMessages {
    public class VoteCard {
        public Vote Vote;

        public VoteCard(Vote vote) {
            Vote = vote;
        }

        public Card[] Build() {
            var builder = new CardBuilder()
                .WithSize(CardSize.Large).WithTheme(CardTheme.Primary)
                .AddModule(new HeaderModuleBuilder().WithText(new PlainTextElementBuilder().WithContent(Vote.Title)))
                .AddModule(new DividerModuleBuilder())
                .AddModule(new CountdownModuleBuilder().WithMode(CountdownMode.Day).WithEndTime(Vote.EndTime));

            foreach (var item in Vote.Items) {
                string itemContent = $"**{item.Title}**";
                if (item.Description != null) {
                    itemContent = $"> **{item.Title}**\n{item.Description}";
                }

                builder.AddModule(new SectionModuleBuilder().WithText(new KMarkdownElementBuilder()
                    .WithContent(itemContent))
                    .WithMode(SectionAccessoryMode.Right)
                    .WithAccessory(new ButtonElementBuilder()
                    .WithClick(ButtonClickEventType.ReturnValue).WithValue(JsonConvert.SerializeObject(new VoteAction {
                        Id = Vote.Id,
                        ItemId = item.Id
                    }))
                    .WithTheme(ButtonTheme.Primary)
                    .WithText(new KMarkdownElementBuilder().WithContent("+1"))));
            }

            builder.AddModule(new ContextModuleBuilder().AddElement(new KMarkdownElementBuilder().WithContent(Vote.Id.ToString())));

            return new Card[] {
                builder.Build()
            };
        }
    }
}
