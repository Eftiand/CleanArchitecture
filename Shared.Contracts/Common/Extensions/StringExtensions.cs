using System.Text.Json;

namespace coaches.Modules.Shared.Application.Common.Extensions;

public static class StringExtensions
{
    public static string ToKebabCase(this string value)
    {
        return string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "-" + x : x.ToString())).ToLower();
    }

    public static T? DeserializeResponseAi<T>(this string? value)
    {
        if (value is null)
        {
            return default;
        }

        string? cleanedJson = value
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();
        return JsonSerializer.Deserialize<T>(cleanedJson)!;
    }
}
