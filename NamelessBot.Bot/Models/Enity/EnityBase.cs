using KaiHeiLa;
using KaiHeiLa.WebSocket;

namespace NamelessBot.Bot.Models.Enity {
    public class EnityBase {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTimeOffset CreatTime { get; set; } = DateTimeOffset.Now;
        public ulong CreatorId { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }

        public ValueTask<IUser> GetCreatorAsync(KaiHeiLaSocketClient socketClient) => socketClient.GetUserAsync(CreatorId);
        public ValueTask<IChannel> GetChannelAsync(KaiHeiLaSocketClient socketClient) => socketClient.GetChannelAsync(ChannelId);
        public SocketGuild GetGuild(KaiHeiLaSocketClient socketClient) => socketClient.GetGuild(GuildId);
    }
}
