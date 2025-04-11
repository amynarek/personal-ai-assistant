using System.Text.Json.Nodes;
using GroqApiLibrary;
using Microsoft.Extensions.Options;
using Telegram.Bot;

public class BotService
{

    private readonly TelegramConfig _telegramConfig;

    private TelegramBotClient? _botClient;
    private GroqService _groqService;

    public BotService(
        IOptions<TelegramConfig> telegramConfig,
        GroqService groqService
    )
    {
        _telegramConfig = telegramConfig.Value;
        _groqService = groqService;
    }

    public async Task Start(CancellationToken ct = default)
    {
        _botClient = new TelegramBotClient(_telegramConfig.ApiKey, cancellationToken: ct);

        var me = await _botClient.GetMe();

        _botClient.OnMessage += OnMessage;

        Console.WriteLine($"Jestem {me.FirstName}. I słucham :)");
    }

    private async Task OnMessage(Telegram.Bot.Types.Message message, Telegram.Bot.Types.Enums.UpdateType type)
    {
        if (message.Text is null)
            return;

        // Console.WriteLine($"Otrzymana wiadomość typu {type} '{message.Text}' in {message.Chat}");

        string response;

        if (message.Text.StartsWith("/"))
        {
            response = ProcessSystemMessage(message.Text);
        }
        else
        {
            response = await ProcessChatMessage(message.Text);
        }

        // Console.WriteLine($"Groq odpowiada: '{groqResponse}'");

        await _botClient.SendMessage(message.Chat, response);
    }

    private Task<string> ProcessChatMessage(string message)
    {
        return _groqService.GetResponse(message);

    }

    private string ProcessSystemMessage(string message)
    {
        var tokens = message.Split(' ');
        var command = tokens[0].ToLowerInvariant();
        var args = tokens[1..];

        return command switch
        {
            "/info" => BuildInfo(),
            "/model" => SetModel(args),
            "/temp" => SetTemp(args),
            _ => "Nieznane polecenie"
        };
    }

    private string BuildInfo()
    {
        return $"Aktualnie używany model '{_groqService.Settings.Model}'; przy temperaturze {_groqService.Settings.Temperature}";
    }

    private Dictionary<string, GroqModel> _allowedModels = new()
    {
        ["llama"] = GroqModel.Llama,
        ["mistral"] = GroqModel.Mistral,
        ["gemma"] = GroqModel.Gemma
    };
    private string SetModel(string[] args)
    {
        if (args == null || args.Length < 1)
            return "Nieprawidłowe parametry polecenia model";

        var desiredModelName = args[0].ToLowerInvariant();

        if (!_allowedModels.ContainsKey(desiredModelName))
            return $"Wybrany model '{desiredModelName}' nie jest dostępny. Obsługiwane opcje: {string.Join(", ", _allowedModels.Keys.ToArray())}";

        _groqService.Settings.Model = _allowedModels[desiredModelName];

        return $"Zmieniono model na '{_groqService.Settings.Model}'";
    }

    private string SetTemp(string[] args)
    {
        if (args == null || args.Length < 1)
            return "Nieprawidłowe parametry polecenia model";

        if (!decimal.TryParse(args[0].ToLowerInvariant(), out var desiredTemperature))
            return $"Nieprawidłowe parametry polecenia model. Nie można konwertować '{args[0]}'";

        if (desiredTemperature < 0 || desiredTemperature >2)            
            return $"Nieprawidłowe parametry polecenia model. Wartość {desiredTemperature} poza wymaganym zakresem <0..2>";

        _groqService.Settings.Temperature = desiredTemperature;

        return $"Zmieniono temperaturę na {_groqService.Settings.Temperature}";
    }
}