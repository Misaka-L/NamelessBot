using Kook;
using Kook.WebSocket;

namespace NamelessBot.Bot.Models.Enity {
    public class EnityBase {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTimeOffset CreatTime { get; set; } = DateTimeOffset.Now;
        public ulong CreatorId { get; set; }
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }

        public Task<IUser> GetCreatorAsync(KookSocketClient socketClient) => socketClient.GetUserAsync(CreatorId);
        public Task<IChannel> GetChannelAsync(KookSocketClient socketClient) => socketClient.GetChannelAsync(ChannelId);
        public SocketGuild GetGuild(KookSocketClient socketClient) => socketClient.GetGuild(GuildId);
    }
}
