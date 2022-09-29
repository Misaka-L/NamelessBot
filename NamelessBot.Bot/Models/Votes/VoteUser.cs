using Kook.WebSocket;
using NamelessBot.Bot.Models.Enity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessBot.Bot.Models.Votes {
    public class VoteUser : EnityBase {
        public VoteUser(KookSocketClient socketClient) { }

        public Guid ItemId;
    }
}
