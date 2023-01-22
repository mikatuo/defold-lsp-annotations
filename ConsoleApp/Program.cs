using App;
using App.Dtos;
using ConsoleApp.Extensions;

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
            //var options = new ProgramArgs(new [] { "1.4.2-beta", "beta" }); // https://d.defold.com/beta/
            //var options = new ProgramArgs(new [] { "1.4.2-alpha", "alpha" }); // https://d.defold.com/alpha/
            var options = new ProgramArgs(args);

            DefoldRelease release = await FindDefoldRelease(options.ReleaseType, options.ReleaseVersion);
            DefoldApiReferenceArchive apiRefArchive = await DownloadDefoldApiRefArchive(release);

            OutputDirectory = $".defold_{release.Version}";
            GenerateHelperLuaModules(apiRefArchive);
            GenerateAnnotations(apiRefArchive);
        }

        static void GenerateHelperLuaModules(DefoldApiReferenceArchive apiRefArchive)
        {
            SaveFile("../defoldy_hashes.lua", GenerateHashesForIncomingMessages(apiRefArchive));
            SaveFile("../defoldy_msgs.lua", GenerateFunctionsForOutgoingMessages(apiRefArchive));
            SaveFile("../defoldy.lua", new [] {
                "local M = require(\"defoldy_msgs\")",
                "M.h = require(\"defoldy_hashes\")",
                "return M",
            });
        }

        static void GenerateAnnotations(DefoldApiReferenceArchive apiRefArchive)
        {
            SaveFile("_readme.txt", new[] {
                $"Annotations for Defold API v{apiRefArchive.Release.Version} ({apiRefArchive.Release.Type.ToString("G").ToLower()}) - {apiRefArchive.Release.Sha1}",
                "Generated with https://github.com/mikatuo/Defold-Lua-Annotations under the MIT license"
            });
            SaveFile("base_defold.lua", GenerateDefoldBaseTypesAnnotations());
            foreach (var filename in apiRefArchive.Files) {
                RawApiReference apiRef = apiRefArchive.ExtractAndDeserialize(filename);
                // clean filenames
                var destFilenameWithoutExtension = apiRef.Info.Name
                    .Replace(" ", "_").Replace("-", "")
                    .ToLower();
                SaveFile($"{destFilenameWithoutExtension}.lua", GenerateLuaAnnotations(apiRef));
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
        
        static IEnumerable<string> GenerateHashesForIncomingMessages(DefoldApiReferenceArchive apiRefArchive)
        {
            var generator = new GenerateLuaModuleForDefoldHashes();
            return generator.GenerateLines(apiRefArchive);
        }

        static IEnumerable<string> GenerateFunctionsForOutgoingMessages(DefoldApiReferenceArchive apiRefArchive)
        {
            var generator = new GenerateLuaModuleForDefoldMessages();
            return generator.GenerateLines(apiRefArchive);
        }

        static IEnumerable<string> GenerateDefoldBaseTypesAnnotations()
            => new GenerateLuaAnnotations().DefoldBaseAnnotations();

        static IEnumerable<string> GenerateLuaAnnotations(RawApiReference apiRef)
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