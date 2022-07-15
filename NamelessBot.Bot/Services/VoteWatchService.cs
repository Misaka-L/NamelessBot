using KaiHeiLa;
using KaiHeiLa.WebSocket;
using Microsoft.Extensions.Hosting;

namespace NamelessBot.Bot.Services {
    public class VoteWatchService : BackgroundService {
        private readonly VoteService _voteService;
        private readonly KaiHeiLaSocketClient _socketClient;

        public VoteWatchService(VoteService voteService, KaiHeiLaSocketClient socketClient) {
            _voteService = voteService;
            _socketClient = socketClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                if (_socketClient.LoginState == LoginState.LoggedIn && _socketClient.ConnectionState == ConnectionState.Connected) {
                    var votes = _voteService.Votes.ToArray();
                    foreach (var vote in votes) {
                        if (vote.EndTime < DateTimeOffset.Now) {
                            await _voteService.EndVote(vote.Id);
                        }
                    }
                }

                await Task.Delay(1000);
            }
        }
    }
}
