using App.Dtos;
using App.Parsers;
using App.Utils;
using System.IO.Compression;
using System.Text.Json;

namespace App
{
    public class DefoldApiReferenceArchive
    {
        public DefoldRelease Release { get; internal set; }
        public string[] Files { get; internal set; }

        byte[]? _source;

        internal DefoldApiReferenceArchive() { } // for unit tests
        public DefoldApiReferenceArchive(DefoldRelease release, byte[]? source)
        {
            _source = source;
            Release = release;
            Files = IgnoreIrrelevantFiles(ListFiles(_source, ".json"));
        }
        
        [Obsolete("TODO: Delete after refactoring")]
        public RawApiReference ExtractAndDeserialize(string filename)
        {
            if (!Files.Contains(filename))
                throw new Exception($"The file '{filename}' is not found in the archive.");
            
            string apiRefJson = UnZipFile(filename);
            var apiRef = DefoldDocsJsonSerializer.Deserialize(apiRefJson);
            return NormalizeAndCleanUp(apiRef);
        }

        public string Extract(string filename)
        {
            if (!Files.Contains(filename))
                throw new Exception($"The file '{filename}' is not found in the archive.");
            return UnZipFile(filename);
        }

        #region Private Methods
        string[] IgnoreIrrelevantFiles(IEnumerable<string> filenames)
        {
            var ignoredFiles = new HashSet<string> {
                "sharedlibrary_doc.json",
                "iap_doc.json",
                "iac_doc.json",
                "camera_doc.json",
                "package_doc.json",
                "string_doc.json",
                "bit_doc.json",
                "coroutine_doc.json",
                "debug_doc.json",
                "engine_doc.json",
                "facebook_doc.json",
                "io_doc.json",
                "math_doc.json",
                "os_doc.json",
                "push_doc.json",
                "table_doc.json",
                "webview_doc.json",
            };
            return filenames
                .Where(filename => !filename.StartsWith("dm")) // ignore C docs
                .Where(filename => !ignoredFiles.Contains(filename))
                .ToArray();
        }

        IEnumerable<string> ListFiles(byte[]? apiReferenceZip, string v)
        {
            if (apiReferenceZip == null)
                return Array.Empty<string>();
            using (var zip = new ZipArchive(new MemoryStream(apiReferenceZip))) {
                return zip.Entries
                    .Where(x => x.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.Name);
            }
        }

        string UnZipFile(string filename)
        {
            using (var zip = new ZipArchive(new MemoryStream(_source!))) {
                var entry = zip.Entries.Single(x => StringComparer.OrdinalIgnoreCase.Equals(x.Name, filename));
                using (var reader = new StreamReader(entry.Open()))
                    return reader.ReadToEnd();
            }
        }

        static RawApiReference Deserialize(string jsonApiRef)
        {
            var options = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
            };
            var apiRef = JsonSerializer.Deserialize<RawApiReference>(jsonApiRef, options);
            if (apiRef == null)
                throw new Exception("Failed to deserialize a Defold API reference file.");
            return apiRef;
        }

        RawApiReference NormalizeAndCleanUp(RawApiReference apiRef)
        {
            apiRef.Info.Description = apiRef.Info.Description;
            foreach (var element in apiRef.Elements) {
                EnhanceMessages(element);
            }
            return apiRef;
        }

        static void CleanUpReturnValues(RawApiRefElement element)
        {
            foreach (var parameter in element.ReturnValues)
                parameter.Description = parameter.Description.Replace("\n", "");
        }
        
        static void EnhanceMessages(RawApiRefElement element)
        {
            if (!StringComparer.OrdinalIgnoreCase.Equals(element.Type, "message"))
                return;
            // automatic identification does not work correctly for animation_done
            // because example text contains "msg.post" in it
            if (element.Name == "animation_done") {
                element.IncomingMessage = true;
                return;
            }
            // the element is a message
            element.OutgoingMessage = element.Description.StartsWith("Post") || element.Examples.Contains("msg.post");
            element.IncomingMessage = !element.OutgoingMessage;
        }
        #endregion
    }
}
