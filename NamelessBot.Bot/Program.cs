using Kook.Commands;
using Kook.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NamelessBot.Bot.Models.Options;
using NamelessBot.Bot.Services;

namespace NamelessBot.Bot {
    internal class Program {
        static Task Main(string[] args) => new Program().MainAsync(args);

        public async Task MainAsync(string[] args) {
            var host = Host.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureServices(ConfigureServices)
                .ConfigureLogging(builder => {
                    builder.AddSimpleConsole(options => {
                        options.IncludeScopes = true;
                        options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
                    }).AddFile();
                });

            await host.RunConsoleAsync();
        }

        private void ConfigureServices(HostBuilderContext context, IServiceCollection services) {
            services.AddSingleton(new KookSocketClient(new KookSocketConfig {
                ConnectionTimeout = 60000
            }))
                .AddSingleton(_ => new CommandService(new CommandServiceConfig {
                }))
                .AddHostedService<HostService>()
                .AddHostedService<CommandHandleService>()
                .AddHostedService<BotMarketStatusService>()
                //.AddSingleton<VoteService>()
                .AddSingleton<MessagebookService>();
                //.AddHostedService<VoteWatchService>();

            services.AddOptions<KookSetting>().Bind(context.Configuration.GetSection("Kook"));
            services.AddOptions<BotMarketSettings>().Bind(context.Configuration.GetSection("BotMarket"));
        }
    }
}