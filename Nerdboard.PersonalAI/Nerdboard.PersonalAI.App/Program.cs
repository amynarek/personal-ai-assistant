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


// var services = new ServiceCollection();

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddOptions<TelegramConfig>()
    .Bind(config.GetSection(TelegramConfig.Section))
    .Validate(x => !x.ApiKey.IsNullOrWhitespace())
    .ValidateOnStart();
builder.Services.AddOptions<GroqConfig>()
    .Bind(config.GetSection(GroqConfig.Section))
    .Validate(x => !x.ApiKey.IsNullOrWhitespace())
    .ValidateOnStart();
builder.Services.AddSingleton<GroqService>();
builder.Services.AddSingleton<BotService>();

// services.AddOptions<TelegramConfig>().Bind(config.GetSection("Telegram"));// ();
// services.AddSingleton<BotService>();
// var svcProvider = services.BuildServiceProvider();

var host = builder.Build();

Console.WriteLine("Started!");

using var cts = new CancellationTokenSource();


var botSvc = host.Services.GetRequiredService<BotService>();
await botSvc.Start(cts.Token);

Console.WriteLine("Press Enter key to exit...");
Console.In.ReadLine();

Console.WriteLine("Run...");
 host.Run();            
Console.WriteLine("Runned...");

cts.Cancel();

