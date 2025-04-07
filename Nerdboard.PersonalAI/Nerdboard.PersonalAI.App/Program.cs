using Microsoft.Extensions.Configuration;
using Telegram.Bot;

IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

using var cts = new CancellationTokenSource();

var bot = new TelegramBotClient(config["Telegram:ApiKey"], cancellationToken: cts.Token);
var me = await bot.GetMe();

bot.OnMessage += OnMessage;

Console.WriteLine($"Jestem {me.FirstName}. I słucham :)");
Console.ReadLine();
cts.Cancel();


async Task OnMessage(Telegram.Bot.Types.Message message, Telegram.Bot.Types.Enums.UpdateType type)
{
    if (message.Text is null) 
        return;	

    Console.WriteLine($"Otrzymana wiadomość typu {type} '{message.Text}' in {message.Chat}");

    await bot.SendMessage(message.Chat, $"Otrzymałem wiadomość: {message.Text}");
}
