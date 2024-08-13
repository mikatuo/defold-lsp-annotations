using App.Utils;
using System.Text.Json.Serialization;

namespace App.Dtos
{
    public class RawApiRefParameter
    {
        public string Name { get; set; }
        [JsonPropertyName("doc")]
        public string Description { get; set; } // TODO: Capitalize
        public string[] Types { get; set; }
        public bool Required => !Optional;
        public bool Optional { get; set; }

        public string TypeAnnotation()
        {
            var typesAnnotation = Types.JoinToString("|");
            if (Optional && !Types.Contains("nil"))
                typesAnnotation += "|nil";
            return typesAnnotation.Length == 0
                ? "any"
                : typesAnnotation;
        }
    }
}
