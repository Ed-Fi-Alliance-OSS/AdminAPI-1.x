using System.Globalization;

namespace EdFi.Ods.Admin.Api.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string input)
            => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);

        public static string ToSingleEntity(this string input)
        {
            return input.Remove(input.Length - 1, 1);
        }
    }
}
