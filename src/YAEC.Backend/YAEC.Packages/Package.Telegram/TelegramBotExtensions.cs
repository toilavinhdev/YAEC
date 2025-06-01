using Microsoft.Extensions.DependencyInjection;

namespace Package.Telegram;

public static class TelegramBotExtensions
{
    public static void AddTelegramBot(this IServiceCollection services)
    {
        services.AddSingleton<ITelegramBotService, TelegramBotService>();
    }
}