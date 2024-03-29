﻿using Kook.WebSocket;
using NamelessBot.Bot.Models.Enity;

namespace NamelessBot.Bot.Models.Votes {
    public class VoteItem : EnityBase {
        public VoteItem(KookSocketClient socketClient) { }

        public string Title { get; set; }
        public string Description { get; set; }
        public long Count { get; set; } = 0;
    }
}
