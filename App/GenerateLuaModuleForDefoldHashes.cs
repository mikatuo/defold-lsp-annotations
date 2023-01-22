using App.Dtos;

namespace App
{
    public class GenerateLuaModuleForDefoldHashes
    {
        public IEnumerable<string> GenerateLines(DefoldApiReferenceArchive apiRefArchive)
        {
            yield return "local M = {}";
            yield return "";
            foreach (var filename in apiRefArchive.Files) {
                RawApiReference apiRef = apiRefArchive.ExtractAndDeserialize(filename);
                // find all incoming message which are received in on_message method in Defold scripts
                RawApiRefElement[] incomingMessages = FindIncomingMessages(apiRef);
                if (incomingMessages.Any()) {
                    foreach (RawApiRefElement message in incomingMessages) {
                        foreach (var line in DescriptionAnnotation(message))
                            yield return line;
                        yield return $"M.{message.Name.ToUpperInvariant()} = hash(\"{message.Name}\")";
                        yield return "";
                    }
                }
            }
            yield return "return M";
        }

        static RawApiRefElement[] FindIncomingMessages(RawApiReference apiRef)
            => apiRef.Elements.Where(x => x.IncomingMessage).ToArray();

        static IEnumerable<string> DescriptionAnnotation(RawApiRefElement message)
            => message.Description.Split("\n").Select(x => x.Trim()).Select(x => $"---{x}");
    }
}
