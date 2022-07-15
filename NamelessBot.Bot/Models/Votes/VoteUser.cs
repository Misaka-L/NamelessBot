using KaiHeiLa.WebSocket;
using NamelessBot.Bot.Models.Enity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessBot.Bot.Models.Votes {
    public class VoteUser : EnityBase {
        public VoteUser(KaiHeiLaSocketClient socketClient) { }

        public Guid ItemId;
    }
}
