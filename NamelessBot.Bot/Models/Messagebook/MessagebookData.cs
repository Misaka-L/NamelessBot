using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessBot.Bot.Models.Messagebook {
    public class MessagebookData {
        public List<Messagebook> Messagebooks { get; set; }
        public Dictionary<ulong, uint[]> GuildManagerRole { get; set; }
    }
}
