using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace EdFi.Ods.AdminApi.Infrastructure.Extensions;

public static class StringExtensions
{
    public static string ToTitleCase(this string input)
        => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);

    public static string ToSingleEntity(this string input)
    {
        return input.Remove(input.Length - 1, 1);
    }

    public static string ToDelimiterSeparated(this IEnumerable<string> inputStrings, string separator = ",")
    {
        var listOfStrings = inputStrings.ToList();

        return listOfStrings.Any()
            ? string.Join(separator, listOfStrings)
            : string.Empty;
    }

    public static object ToJsonObjectResponseDeleted(this string input)
    {
        return new { title = $"{input} deleted successfully" };
    }

    public static string ToPascalCase(this string input)
    {
        var matches = Regex.Match(input, "^(?<word>^[a-z]+|[A-Z]+|[A-Z][a-z]+)+$");
        var groupWords = matches.Groups["word"];

        var thread = Thread.CurrentThread.CurrentCulture.TextInfo;
        var stringBuilder = new StringBuilder();
        foreach (var captureWord in groupWords.Captures.Cast<Capture>())
            stringBuilder.Append(thread.ToTitleCase(captureWord.Value.ToLower()));
        return stringBuilder.ToString();
    }

}


