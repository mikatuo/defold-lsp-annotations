using Core;
using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace App
{
    public class DefoldApiReferenceArchive
    {
        public string[] Files { get; internal set; }

        byte[]? _source;

        internal DefoldApiReferenceArchive() { } // for unit tests
        public DefoldApiReferenceArchive(byte[]? source)
        {
            _source = source;
            Files = ListFiles(_source, ".json");
        }

        public DefoldApiReference Extract(string filename)
        {
            if (!Files.Contains(filename))
                throw new Exception($"The file '{filename}' is not found in the archive.");
            
            string apiRefJson = StripHtmlMarkup(UnZipFile(filename));
            var apiRef = Deserialize(apiRefJson);
            NormalizeAndCleanUp(apiRef);
            return apiRef;
        }

        #region Private Methods
        string[] ListFiles(byte[]? apiReferenceZip, string v)
        {
            if (apiReferenceZip == null)
                return Array.Empty<string>();
            using (var zip = new ZipArchive(new MemoryStream(apiReferenceZip))) {
                return zip.Entries
                    .Where(x => x.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    .Where(x => !x.Name.StartsWith("dm")) // ignore C docs
                    .Select(x => x.Name)
                    .ToArray();
            }
        }

        string UnZipFile(string filename)
        {
            using (var zip = new ZipArchive(new MemoryStream(_source))) {
                var entry = zip.Entries.Single(x => StringComparer.OrdinalIgnoreCase.Equals(x.Name, filename));
                using (var reader = new StreamReader(entry.Open()))
                    return reader.ReadToEnd();
            }
        }

        static DefoldApiReference Deserialize(string jsonApiRef)
        {
            var options = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
            };
            var apiRef = JsonSerializer.Deserialize<DefoldApiReference>(jsonApiRef, options);
            if (apiRef == null)
                throw new Exception("Failed to deserialize a Defold API reference file.");
            return apiRef;
        }

        void NormalizeAndCleanUp(DefoldApiReference apiRef)
        {
            apiRef.Info.Description = apiRef.Info.Description;
            foreach (var element in apiRef.Elements) {
                CleanUpParameters(element);
                CleanUpReturnValues(element);
                element.Description = element.Description;
            }
        }
        
        static void CleanUpParameters(ApiRefElement element)
        {
            foreach (var parameter in element.Parameters) {
                if (parameter.Name.StartsWith('[')) {
                    parameter.Name = parameter.Name.Trim('[', ']');
                    parameter.Optional = true;
                }
                parameter.Description = parameter.Description.Replace("\n", "");
            }
        }

        static void CleanUpReturnValues(ApiRefElement element)
        {
            foreach (var parameter in element.ReturnValues) {
                parameter.Description = parameter.Description.Replace("\n", "");
            }
        }

        static string StripHtmlMarkup(string value)
        {
            // strip HTML markup
            var regex = new Regex(@"<[^>]+>");
            return regex.Replace(value, "");
        }
        #endregion
    }
}
