using Core;

namespace App
{
    public class GenerateLuaModuleForDefoldHashes
    {
        public IEnumerable<string> GenerateLines(DefoldApiReferenceArchive apiRefArchive)
        {
            yield return "local M = {}";
            yield return "";
            foreach (var filename in apiRefArchive.Files) {
                DefoldApiReference apiRef = apiRefArchive.Extract(filename);
                // find all incoming message which are received in on_message method in Defold scripts
                ApiRefElement[] incomingMessages = FindIncomingMessages(apiRef);
                if (incomingMessages.Any()) {
                    foreach (ApiRefElement message in incomingMessages) {
                        foreach (var line in DescriptionAnnotation(message))
                            yield return line;
                        yield return $"M.{message.Name.ToUpperInvariant()} = hash(\"{message.Name}\")";
                        yield return "";
                    }
                }
            }
            yield return "return M";
        }

        static ApiRefElement[] FindIncomingMessages(DefoldApiReference apiRef)
            => apiRef.Elements.Where(x => x.IncomingMessage).ToArray();

        static IEnumerable<string> DescriptionAnnotation(ApiRefElement message)
            => message.Description.Split("\n").Select(x => x.Trim()).Select(x => $"---{x}");
    }
}
