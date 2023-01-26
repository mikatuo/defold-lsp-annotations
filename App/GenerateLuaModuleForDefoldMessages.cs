using App.Dtos;

namespace App
{
    public class GenerateLuaModuleForDefoldMessages
    {
        HashSet<string> _definedHashes = new HashSet<string>();

        // TODO: refactor
        public IEnumerable<string> GenerateLines(DefoldApiReferenceArchive apiRefArchive)
        {
            // "enable" and "disable" messages are defined twice (in "go" and "collectionproxy" namespaces)
            // disable duplicate diagnostics so that Lua (LSP) extension in VS Code does not show these problems
            yield return "---@diagnostic disable: duplicate-set-field";
            yield return "local M = {}";
            yield return "";
            foreach (var filename in apiRefArchive.Files) {
                RawApiReference apiRef = apiRefArchive.ExtractAndDeserialize(filename);
                // find all outgoing message which can be used in msg.post
                RawApiRefElement[] outgoingMessages = FindOutgoingMessages(apiRef);
                if (outgoingMessages.Any()) {
                    yield return $"---{apiRef.Info.Brief}";
                    yield return "";
                    foreach (RawApiRefElement msg in outgoingMessages) {
                        if (HashIsNotAddedYet(msg.Name)) {
                            MarkThatHashHasBeenAdded(msg.Name);
                            yield return $"local h_{msg.Name} = hash(\"{msg.Name}\")";
                        }
                        yield return "";

                        if (msg.Name == "acquire_input_focus" || msg.Name == "release_input_focus") {
                            yield return $"---Docs: https://defold.com/ref/stable/{apiRef.Info.Namespace}/?q={msg.Name}#{msg.Name}";
                            yield return "---";
                            yield return $"---Namespace: {apiRef.Info.Namespace}";
                            yield return "---";
                            yield return $"---{msg.Brief}";
                            yield return $"---@param receiver hash|string|url|nil";
                            yield return $"---@overload fun()";
                            yield return $"function M.{msg.Name}(receiver)";
                            yield return $"    msg.post(receiver or \".\", h_{msg.Name})";
                            yield return $"end";
                            yield return "";
                            continue; // go to the next message
                        }

                        string formattedMessageTypes = FormattedMessageTypes(msg);

                        yield return $"---Docs: https://defold.com/ref/stable/{apiRef.Info.Namespace}/?q={msg.Name}#{msg.Name}";
                        yield return "---";
                        yield return $"---Namespace: {apiRef.Info.Namespace}";
                        yield return "---";
                        yield return $"---{msg.Brief}";
                        yield return $"---@param receiver hash|string|url";
                        if (HasEmptyMessage(formattedMessageTypes)) {
                            yield return $"function M.{msg.Name}(receiver)";
                            yield return $"    msg.post(receiver, h_{msg.Name})";
                            yield return $"end";
                        } else {
                            //yield return $"---@param message {{{formattedMessageTypes}}}";
                            yield return $"---@param message {msg.Name}_msg";
                            if (msg.Parameters.All(x => x.Optional))
                                yield return $"---@overload fun(receiver: hash|string|url)";
                            yield return $"function M.{msg.Name}(receiver, message)";
                            yield return $"    msg.post(receiver, h_{msg.Name}, message)";
                            yield return $"end";
                        }
                        yield return "";
                    }
                }
            }
            yield return "return M";
        }

        #region Private Methods
        static RawApiRefElement[] FindOutgoingMessages(RawApiReference apiRef)
            => apiRef.Elements.Where(x => x.OutgoingMessage).ToArray();

        static string FormattedMessageTypes(RawApiRefElement message)
        {
            var parameters = message.Parameters.Select(x => {
                var types = x.TypeAnnotation();
                return $"{x.Name.Trim('[', ']')}:{types}";
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
