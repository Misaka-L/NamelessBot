using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NamelessBot.Bot.Models.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamelessBot.Bot.Services {
    public class BotMarketStatusService : BackgroundService {
        private HttpClient _client = new HttpClient();

        private readonly IOptions<BotMarketSettings> _options;
        private readonly ILogger<BotMarketStatusService> _logger;

        public BotMarketStatusService(IOptions<BotMarketSettings> options, ILogger<BotMarketStatusService> logger) {
            _options = options;
            _logger = logger;

            if (_options.Value.Enabled && _options.Value.BotMarketId.HasValue) {
                _client.DefaultRequestHeaders.Add("uuid", _options.Value.BotMarketId.ToString());
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            if (_options.Value.Enabled) {
                while (!stoppingToken.IsCancellationRequested) {
                    try {
                        _logger.LogInformation("正在更新 BotMarket 状态...");
                        var response = await _client.GetAsync("http://bot.gekj.net/api/v1/online.bot");
                        if (response.IsSuccessStatusCode) {
                            _logger.LogInformation("更新 BotMarket 状态成功");
                        } else {
                            throw new Exception($"{response.StatusCode}: {response.ReasonPhrase}");
                        }
                    } catch (Exception ex) {
                        _logger.LogError(ex, "无法更新 BotMarket 状态");
                    }
                    
                    await Task.Delay(TimeSpan.FromMinutes(25));
                }
            }
        }
    }
}
