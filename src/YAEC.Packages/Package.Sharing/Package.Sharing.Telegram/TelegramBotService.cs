using System.Net.Http.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Package.Sharing.Telegram;

public interface ITelegramBotService
{
    Task<object?> GetUpdatesAsync(CancellationToken cancellationToken = default);

    Task<Message> SendMessageAsync(string message, CancellationToken cancellation = default);
}

public class TelegramBotService : ITelegramBotService
{
    private readonly ITelegramBotClient _client;
    
    private readonly TelegramBotOptions _options;
    
    private readonly HttpClient _httpClient;
    
    public TelegramBotService(TelegramBotOptions options, HttpClient httpClient)
    {
        _options = options;
        _httpClient = httpClient;
        _client = new TelegramBotClient(options.Token);
    }

    public async Task<object?> GetUpdatesAsync(CancellationToken cancellationToken = default)
    {
        var apiUrl = $"https://api.telegram.org/bot{_options.Token}/getUpdates";
        var httpResponse = await _httpClient.GetAsync(apiUrl, cancellationToken);
        var data = await httpResponse.Content.ReadFromJsonAsync<object>(cancellationToken: cancellationToken);
        // var recentlyChat = data?.result[0].message.chat;
        return data;
    }

    public async Task<Message> SendMessageAsync(string message, CancellationToken cancellation = default)
    {
        return await _client.SendMessage(
            chatId: _options.ChatId,
            text: message,
            parseMode: ParseMode.Html,
            cancellationToken: cancellation);
    }
}