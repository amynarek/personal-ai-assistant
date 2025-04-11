using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;

Console.WriteLine("Service is starting...");

IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();


var services = new ServiceCollection();

// var builder = Host.CreateApplicationBuilder(args);
// builder.Services.AddOptions<TelegramConfig>().Bind(config.GetSection("Telegram"));// ();
// builder.Services.AddSingleton<BotService>();

services.AddOptions<TelegramConfig>().Bind(config.GetSection("Telegram"));// ();
services.AddSingleton<BotService>();
var svcProvider = services.BuildServiceProvider();

Console.WriteLine("Started!");

using var cts = new CancellationTokenSource();


var botSvc = svcProvider.GetRequiredService<BotService>();
await botSvc.Start(cts.Token);

Console.WriteLine("Press Enter key to exit...");
Console.In.ReadLine();

cts.Cancel();

// host.Run();            
