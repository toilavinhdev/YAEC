using System.ComponentModel;

namespace YAEC.Shared.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum input)
    {
        var attr = input
            .GetType()
            .GetField(input.ToString())!
            .GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
        return attr?.Description ?? string.Empty;
    }
}