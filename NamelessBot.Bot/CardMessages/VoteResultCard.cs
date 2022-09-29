using Kook;
using NamelessBot.Bot.Models;

namespace NamelessBot.Bot.CardMessages {
    public class VoteResultCard {
        public Vote Vote;

        public VoteResultCard(Vote vote) {
            Vote = vote;
        }

        public Card[] Build() {
            var builder = new CardBuilder()
                .WithTheme(CardTheme.Success).WithSize(CardSize.Large)
                .AddModule(new HeaderModuleBuilder().WithText(new PlainTextElementBuilder().WithContent(Vote.Title)))
                .AddModule(new DividerModuleBuilder());

            foreach (var item in Vote.Items) {
                string metion = "> ";
                if (Vote.Users.Where(u => u.ItemId == item.Id).Count() == 0) metion += "大家过于默契以至于没人选这个选项";
                foreach (var user in Vote.Users.Where(u => u.ItemId == item.Id)) {
                    metion += $"(met){user.Id}(met) ";
                }

                if (item.Description != null) {
                    builder.AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder()
                        .WithColumnCount(2)
                        .AddField(new KMarkdownElementBuilder().WithContent($"> **{item.Title}**\n{item.Description}"))
                        .AddField(new KMarkdownElementBuilder().WithContent($"票数\n{item.Count}"))));
                    builder.AddModule(new ContextModuleBuilder().AddElement(new KMarkdownElementBuilder().WithContent(metion)));
                } else {
                    builder.AddModule(new SectionModuleBuilder().WithText(new ParagraphStructBuilder()
                        .WithColumnCount(2)
                        .AddField(new KMarkdownElementBuilder().WithContent($"> **{item.Title}**"))
                        .AddField(new KMarkdownElementBuilder().WithContent($"{item.Count}"))));
                    builder.AddModule(new ContextModuleBuilder().AddElement(new KMarkdownElementBuilder().WithContent(metion)));
                }
            }

            return new Card[] {
                builder.Build()
            };
        }
    }
}
