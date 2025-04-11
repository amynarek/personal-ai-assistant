using System.Text.Json.Nodes;
using GroqApiLibrary;
using Microsoft.Extensions.Options;

public enum GroqModel
{
    Llama,
    Mistral,
    Gemma
}

public class GroqSettings
{
    public GroqModel Model { get; set; } = GroqModel.Llama;

    private decimal _temperature;
    public decimal Temperature
    {
        get => _temperature;

        set
        {
            if (value > 2 || value < 0)
                throw new ArgumentOutOfRangeException(nameof(Temperature), value, "Temperature must be in range <0..2>");
            _temperature = value;
        }
    }

    public string ModelName => Model switch
    {
        GroqModel.Llama => "llama-3.3-70b-versatile",
        GroqModel.Mistral => "mistral-saba-24b",
        GroqModel.Gemma => "gemma2-9b-it",
        _ => "llama-3.3-70b-versatile"
    };
}



public class GroqService
{
    const string AI_SYSTEM_CTX = "Jesteś raperem, który na każde pytanie odpowiada rymując";

    const int AI_MAX_TOKENS = 150;

    private readonly GroqConfig _groqConfig;
    private readonly GroqApiClient _groqClient;

    public readonly GroqSettings Settings = new();

    public GroqService(
        IOptions<GroqConfig> groqConfig
    )
    {
        _groqConfig = groqConfig.Value;
        _groqClient = new GroqApiClient(_groqConfig.ApiKey);
    }

    public Task<string> GetResponse(string message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        return SendInSystemContext(message);
    }

    private async Task<string> SendInSystemContext(string message)
    {
        var request = new JsonObject
        {
            ["model"] = Settings.ModelName,
            ["temperature"] = Settings.Temperature,
            ["max_completion_tokens"] = AI_MAX_TOKENS,
            ["messages"] = new JsonArray
            {
                new JsonObject
                {
                    ["role"] = "system",
                    ["content"] = AI_SYSTEM_CTX
                },
                new JsonObject
                {
                    ["role"] = "user",
                    ["content"] = message
                }
            }
        };

        var result = await _groqClient.CreateChatCompletionAsync(request);

        return result?["choices"]?[0]?["message"]?["content"]?.ToString() ?? string.Empty;

    }



}