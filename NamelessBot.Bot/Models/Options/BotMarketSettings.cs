using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessBot.Bot.Models.Options {
    public class BotMarketSettings {
        public bool Enabled { get; set; } = false;
        public Guid? BotMarketId { get; set; }
    }
}
