using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Package.Shared.Extensions;

public static class StringExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };
    
    public static string? NullIfEmpty(this string input) => input == string.Empty ? null : input;
    
    public static Guid ToGuid(this string input) => Guid.TryParse(input, out var result) ? result : Guid.Empty;

    public static int ToInt(this string input) => int.TryParse(input, out var result) ? result : 0;

    public static long ToLong(this string input) => long.TryParse(input, out var result) ? result : 0;

    public static double ToDouble(this string input) => double.TryParse(input, out var result) ? result : 0;

    public static bool ToBool(this char value) => value switch
    {
        '1' => true,
        '0' => false,
        _ => throw new ArgumentOutOfRangeException(value.ToString())
    };

    public static string ToJson<T>(this T input) => JsonSerializer.Serialize(input, JsonSerializerOptions);

    public static T ToObject<T>(this string json) => JsonSerializer.Deserialize<T>(json, JsonSerializerOptions)!;
    
    public static T ToEnum<T>(this string input)
    {
        return (Enum.TryParse(typeof(T), input, out var result) ? (T)result : default)!;
    }
    
    public static string RandomString(int length, string? pattern = null)
    {
        const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        return new string(Enumerable.Repeat(pattern ?? characters, length)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }
    
    public static string ToUnderscoreCase(this string input) => input.ToSnakeCase('_');

    public static string ToKebabCase(this string input) => input.ToSnakeCase('-');

    private static string ToSnakeCase(this string input, char separator)
    {
        if (string.IsNullOrEmpty(input)) return input;
        var inspect = input.Select(
            (x, idx) => idx > 0 && char.IsUpper(x)
                ? $"{separator}{x}"
                : string.IsNullOrWhiteSpace(x.ToString())
                    ? string.Empty
                    : x.ToString());
        return string.Concat(inspect).ToLower();
    }
    
    public static Stream ToMemoryStream(this string @this)
    {
        Encoding encoding = Activator.CreateInstance<ASCIIEncoding>();
        return new MemoryStream(encoding.GetBytes(@this));
    }
    
    public static string ToSha256(this string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        var data = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var stringBuilder = new StringBuilder();
        foreach (var byteCode in data)
            stringBuilder.Append(byteCode.ToString("X2"));
        return stringBuilder.ToString();
    }
    
    public static string Slugify(this string input)
    {
        var regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
        var slug = input
            .Normalize(NormalizationForm.FormD)
            .Trim()
            .ToLower();
        slug = regex
            .Replace(slug, string.Empty)
            .Replace('\u0111', 'd')
            .Replace('\u0110', 'D')
            .Replace(",", "-")
            .Replace(".", "-")
            .Replace("!", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace(";", "-")
            .Replace("/", "-")
            .Replace("%", "ptram")
            .Replace("&", "va")
            .Replace("?", "")
            .Replace('"', '-')
            .Replace(' ', '-');
        return slug;
    }
    
    public static string RemoveDiacritics(this string input)
    {
        var normalizedString = input.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();
        foreach (var t in normalizedString)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(t);
            if (uc != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(t);
            }
        }
        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
    
    public static void SaveAs(this string input, string fileName, bool append = false)
    {
        using TextWriter tw = new StreamWriter(fileName, append);
        tw.Write(input);
    }
}