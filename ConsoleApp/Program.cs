using App;
using ConsoleApp.Extensions;
using Core;
using System.Numerics;
using System;
using System.Xml.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        static string OutputDirectory = ".defold";

        static async Task Main(string[] args)
        {
            ///////////////////////////////////////////
            // uncomment a following line to generate /
            // annotations for a specific version     /
            ///////////////////////////////////////////
            //var options = new ProgramArgs(new [] { "1.4.1", "stable" }); // https://d.defold.com/stable/
            //var options = new ProgramArgs(new [] { "1.4.2-alpha", "alpha" }); // https://d.defold.com/alpha/
            
            var options = new ProgramArgs(args);
            DefoldRelease release = await FindDefoldRelease(options.ReleaseType, options.ReleaseVersion);
            DefoldApiReferenceArchive apiRefArchive = await DownloadDefoldApiRefArchive(release);

            SaveFile("_readme.txt", new [] {
                $"Annotations for Defold API v{release.Version} ({release.Type.ToString("G").ToLower()})",
            });
            SaveFile("base_defold.lua", GenerateDefoldBaseTypesAnnotations());
            SaveFile("../defold_hashes.lua", GenerateHashesForIncomingMessages(apiRefArchive));
            SaveFile("../defold_msgs.lua", GenerateFunctionsForOutgoingMessages(apiRefArchive));
            foreach (var filename in apiRefArchive.Files) {
                DefoldApiReference apiRef = apiRefArchive.Extract(filename);
                var destinationFilename = $"{apiRef.Info.Name.ToLower()}.lua";
                SaveFile(destinationFilename, GenerateLuaAnnotations(apiRef));
            }
        }

        #region Private Methods
        static async Task<DefoldRelease> FindDefoldRelease(ReleaseType type, string version)
        {
            var releases = await new ListDefoldReleases(type).DownloadAsync();
            // TODO: get Sha1 of the latest Defold release with a different HTTP call
            if (StringComparer.OrdinalIgnoreCase.Equals(version, "latest"))
                return releases.OrderByDescending(x => x.Version).First();

            var release = releases.SingleOrDefault(x => x.Version == version);
            if (release == null)
                throw new Exception($"Can not find a release with the version {version}");
            return release;
        }

        static async Task<DefoldApiReferenceArchive> DownloadDefoldApiRefArchive(DefoldRelease release)
            => await new DownloadDefoldApiReferenceArchive().DownloadAsync(release);
        
        // TODO: WIP: experimenting... when done move somewhere else, refactor, add tests
        static IEnumerable<string> GenerateHashesForIncomingMessages(DefoldApiReferenceArchive apiRefArchive)
        {
            yield return "local M = {}";
            yield return "";
            foreach (var filename in apiRefArchive.Files) {
                DefoldApiReference apiRef = apiRefArchive.Extract(filename);
                // find all incoming message, ignore message_id-s that are for msg.post
                ApiRefElement[] incomingMessages = apiRef.Elements.Where(x => x.IncomingMessage).ToArray();
                if (incomingMessages.Any()) {
                    //yield return $"---{apiRef.Info.Brief}";
                    //yield return $"M.{apiRef.Info.Namespace} = {{}}";
                    //yield return "";
                    foreach (ApiRefElement message in incomingMessages) {
                        var descriptionAnnotation = message.Description.Split("\n").Select(x => x.Trim()).Select(x => $"---{x}");
                        foreach (var line in descriptionAnnotation)
                            yield return line;
                        //yield return $"M.{apiRef.Info.Namespace}.{message.Name} = hash(\"{message.Name}\")";
                        yield return $"M.{message.Name} = hash(\"{message.Name}\")";
                        yield return "";
                    }
                }
            }
            yield return "return M";
        }

        static IEnumerable<string> GenerateFunctionsForOutgoingMessages(DefoldApiReferenceArchive apiRefArchive)
        {
            var definedHashes = new HashSet<string>();

            yield return "local M = {}";
            yield return "";
            foreach (var filename in apiRefArchive.Files) {
                DefoldApiReference apiRef = apiRefArchive.Extract(filename);
                // find all outgoing message, they are used in msg.post
                ApiRefElement[] outgoingMessages = apiRef.Elements.Where(x => x.OutgoingMessage).ToArray();
                if (outgoingMessages.Any()) {
                    yield return $"---{apiRef.Info.Brief}";
                    yield return "";
                    foreach (ApiRefElement message in outgoingMessages) {
                        var parameters = message.Parameters.Select(x => {
                            var types = x.TypeAnnotation();
                            return $"{x.Name}:{types}";
                        });
                        var formattedParameters = string.Join(", ", parameters);

                        if (!definedHashes.Contains(message.Name)) {
                            definedHashes.Add(message.Name);
                            yield return $"local h_{message.Name} = hash(\"{message.Name}\")";
                        }
                        yield return "";
                        yield return $"---Docs: https://defold.com/ref/stable/{apiRef.Info.Namespace}/?q={message.Name}#{message.Name}";
                        yield return "---";
                        yield return $"---Namespace: {apiRef.Info.Namespace}";
                        yield return "---";
                        yield return $"---{message.Brief}";
                        yield return $"---@param receiver hash|string|url";
                        if (formattedParameters == "") {
                            yield return $"function M.{message.Name}(receiver)";
                            //yield return $"function M.{apiRef.Info.Namespace}.{message.Name}(receiver)";
                            yield return $"    msg.post(receiver, h_{message.Name})";
                            yield return $"end";
                        } else {
                            yield return $"---@param message {{{formattedParameters}}}";
                            if (message.Parameters.All(x => x.Optional))
                                yield return $"---@overload fun(receiver: hash|string|url)";
                            //yield return $"function M.{apiRef.Info.Namespace}.{message.Name}(receiver, message)";
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

        static IEnumerable<string> GenerateDefoldBaseTypesAnnotations()
            => new GenerateLuaAnnotations().DefoldBaseAnnotations();

        static IEnumerable<string> GenerateLuaAnnotations(DefoldApiReference apiRef)
            => new GenerateLuaAnnotations().ForApiReference(apiRef);

        static void SaveFile(string filename, IEnumerable<string> lines)
        {
            Directory.CreateDirectory(OutputDirectory);
            var path = Path.Combine(OutputDirectory, filename);
            using (var f = new StreamWriter(path)) {
                foreach (var line in lines)
                    f.WriteLine(line);
            }
        }
        #endregion
    }
}