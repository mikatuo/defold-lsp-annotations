using App.Infrastructure;
using System.Text.Json.Serialization;

namespace Core
{
    public class DefoldApiReference
    {
        public ApiRefElement[] Elements { get; set; }
        public ApiRefInfo Info { get; set; }
    }

    public class ApiRefInfo
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Brief { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string File { get; set; }
        public string Group { get; set; }

        public IEnumerable<string> DescriptionAnnotation()
            => Description.Split("\n")
                .Select(x => x.Trim())
                .Select(x => $"---{x}");
    }

    public class ApiRefReturnValue
    {
        public string Name { get; set; }
        [JsonPropertyName("doc")]
        public string Description { get; set; }
        public string[] Types { get; set; }

        public string ToAnnitation()
        {
            var typesAnnotation = Types.JoinToString("|");
            return typesAnnotation.Length == 0
                ? "any"
                : typesAnnotation;
        }
    }
}
