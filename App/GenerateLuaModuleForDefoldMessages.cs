using Core;

namespace App
{
    public class GenerateLuaModuleForDefoldMessages
    {
        HashSet<string> _definedHashes = new HashSet<string>();

        public IEnumerable<string> GenerateLines(DefoldApiReferenceArchive apiRefArchive)
        {
            // "enable" and "disable" messages are defined twice (in "go" and "collectionproxy" namespaces)
            // disable duplicate diagnostics so that Lua (LSP) extension in VS Code does not show these problems
            yield return "---@diagnostic disable: duplicate-set-field";
            yield return "local M = {}";
            yield return "";
            foreach (var filename in apiRefArchive.Files) {
                DefoldApiReference apiRef = apiRefArchive.Extract(filename);
                // find all outgoing message which can be used in msg.post
                ApiRefElement[] outgoingMessages = FindOutgoingMessages(apiRef);
                if (outgoingMessages.Any()) {
                    yield return $"---{apiRef.Info.Brief}";
                    yield return "";
                    foreach (ApiRefElement message in outgoingMessages) {
                        if (HashIsNotAddedYet(message.Name)) {
                            MarkThatHashHasBeenAdded(message.Name);
                            yield return $"local h_{message.Name} = hash(\"{message.Name}\")";
                        }
                        yield return "";
                        yield return $"---Docs: https://defold.com/ref/stable/{apiRef.Info.Namespace}/?q={message.Name}#{message.Name}";
                        yield return "---";
                        yield return $"---Namespace: {apiRef.Info.Namespace}";
                        yield return "---";
                        yield return $"---{message.Brief}";
                        yield return $"---@param receiver hash|string|url";

                        string formattedMessageTypes = FormattedMessageTypes(message);
                        if (HasEmptyMessage(formattedMessageTypes)) {
                            yield return $"function M.{message.Name}(receiver)";
                            yield return $"    msg.post(receiver, h_{message.Name})";
                            yield return $"end";
                        } else {
                            yield return $"---@param message {{{formattedMessageTypes}}}";
                            if (message.Parameters.All(x => x.Optional))
                                yield return $"---@overload fun(receiver: hash|string|url)";
                            yield return $"function M.{message.Name}(receiver, message)";
                            yield return $"    msg.post(receiver, h_{message.Name}, message)";
                            yield return $"end";
                        }
                        yield return "";
                    }
                }
            }
            yield return "return M";
        }

        #region Private Methods
        static ApiRefElement[] FindOutgoingMessages(DefoldApiReference apiRef)
            => apiRef.Elements.Where(x => x.OutgoingMessage).ToArray();

        static string FormattedMessageTypes(ApiRefElement message)
        {
            var parameters = message.Parameters.Select(x => {
                var types = x.TypeAnnotation();
                return $"{x.Name}:{types}";
            });
            var formattedParameters = string.Join(", ", parameters);
            return formattedParameters;
        }

        bool HashIsNotAddedYet(string name)
            => !_definedHashes.Contains(name);

        void MarkThatHashHasBeenAdded(string name)
            => _definedHashes.Add(name);

        static bool HasEmptyMessage(string formattedMessageTypes)
            => string.IsNullOrEmpty(formattedMessageTypes);
        #endregion
    }
}
