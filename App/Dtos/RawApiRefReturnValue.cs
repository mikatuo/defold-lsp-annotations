using App.Utils;
using System.Text.Json.Serialization;

namespace App.Dtos
{
    public class RawApiRefReturnValue
    {
        public string Name { get; set; }
        [JsonPropertyName("doc")]
        public string Description { get; set; }
        public string[] Types { get; set; }

        public string ToAnnotation()
        {
            var typesAnnotation = Types.JoinToString("|");
            return typesAnnotation.Length == 0
                ? "any"
                : typesAnnotation;
        }
    }
}
