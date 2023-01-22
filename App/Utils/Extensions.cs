using App.Dtos;
using System.Text.RegularExpressions;

namespace App.Utils
{
    static class Extensions
    {
        public static string JoinToString(this IEnumerable<string> collection, string delimiter)
            => string.Join(delimiter, collection);

        static readonly Regex _htmlMarkupRegex = new Regex(@"<(?<tag>/?[^>]+)>");
        public static string StripHtmlMarkup(this string value)
        {
            value = value.Replace("</li>\n<li>", "\n\n");
            value = _htmlMarkupRegex.Replace(value, match => {
                switch (match.Value) {
                    // keep <code> and </code> tags
                    case "<code>" or "</code>":
                        return match.Value;
                    default:
                        return "";
                }
            });
            return value;
        }

        public static bool EqualsIgnoreCase(this string value, string other)
            => StringComparer.OrdinalIgnoreCase.Equals(value, other);

        public static IEnumerable<RawApiRefElement> Where(this RawApiRefElement[] elements, string type)
            => elements.Where(x => x.Type.EqualsIgnoreCase(type));
    }
}
