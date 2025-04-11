using Microsoft.Extensions.Options;
using Telegram.Bot;

public class BotService
{

    private readonly TelegramConfig _telegramConfig;

    private TelegramBotClient _botClient;

    public BotService(
        IOptions<TelegramConfig> telegramConfig
    )
    {
        _telegramConfig = telegramConfig.Value;
    }

    public async Task Start(CancellationToken ct = default)
    {
        if (_telegramConfig.ApiKey.IsNullOrWhitespace())
            throw new InvalidOperationException("Set Telegram:ApiKey in env variables");


        _botClient = new TelegramBotClient(_telegramConfig.ApiKey, cancellationToken: ct);
        var me = await _botClient.GetMe();

        _botClient.OnMessage += OnMessage;

        Console.WriteLine($"Jestem {me.FirstName}. I słucham :)");
    }

    private async Task OnMessage(Telegram.Bot.Types.Message message, Telegram.Bot.Types.Enums.UpdateType type)
    {
        if (message.Text is null) 
            return;	

        Console.WriteLine($"Otrzymana wiadomość typu {type} '{message.Text}' in {message.Chat}");

        await _botClient.SendMessage(message.Chat, $"Otrzymałem wiadomość: {message.Text}");
    }

}