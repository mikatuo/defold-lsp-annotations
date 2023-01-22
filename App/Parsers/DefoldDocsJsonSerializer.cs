using App.Dtos;
using System.Text.Json;

namespace App.Parsers
{
    class DefoldDocsJsonSerializer
    {
        public static RawApiReference Deserialize(string json)
        {
            var options = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
            };
            var apiRef = JsonSerializer.Deserialize<RawApiReference>(json, options);
            if (apiRef == null)
                throw new Exception("Failed to deserialize Defold API from JSON.");
            return apiRef;
        }
    }
}
