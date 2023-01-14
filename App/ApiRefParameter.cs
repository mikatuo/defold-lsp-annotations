using System.Text.Json.Serialization;

namespace Core
{
    public class ApiRefParameter
    {
        public string Name { get; set; }
        [JsonPropertyName("doc")]
        public string Description { get; set; } // TODO: Capitalize
        public string[] Types { get; set; }
        public bool Required => !Optional;
        public bool Optional { get; set; }

        public string TypeAnnotation()
        {
            var typesAnnotation = string.Join("|", Types);
            if (Optional && !Types.Contains("nil"))
                    typesAnnotation += "|nil";
            return typesAnnotation.Length == 0
                ? "any"
                : typesAnnotation;
        }

        public string ToAnnotation()
            => $"---@param {Name} {TypeAnnotation()} {Description}";
    }
}
