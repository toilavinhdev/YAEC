using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Package.Serilog;

public static class LoggerExtensions
{
    public static void LogMethodInformation(
        this ILogger logger,
        object? parameters = null,
        [CallerMemberName] string? callerMemberName = null)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("{MethodName}");
        if (parameters is not null) stringBuilder.Append(": {@Parameters}");
        logger.LogInformation(stringBuilder.ToString(), callerMemberName, parameters);
    }
}