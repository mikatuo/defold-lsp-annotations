using App;
using App.Dtos;
using ConsoleApp.Extensions;
using System.IO.Compression;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // BUGS:
            // ---
            // replace "-" in parameter names with "_" -> tilemap.lua -> tilemap.set_tile > transform-bitmask
            // ---
            // luasocket.lua -> connected:setpeername("*")
            // ---
            // DOT NOT CREATE EMPTY GLOBAL MODULES, in example:
            // ---@class builtins
            // builtins = {}
            // TODO: parse local vs global functions, if module has no local functions then do not generate "local builtins = {}"

            ///////////////////////////////////////////
            // uncomment a following line to generate /
            // annotations for a specific version     /
            ///////////////////////////////////////////
            var options = new ProgramArgs(new[] { "1.5.0", "stable" });
            //var options = new ProgramArgs(new[] { "1.4.4-beta", "beta" })
            //var options = new ProgramArgs(new [] { "1.4.2-alpha", "alpha" });
            //var options = new ProgramArgs(args);

            // TODO: parse examples

            DefoldRelease release = await FindDefoldRelease(options.ReleaseType, options.ReleaseVersion);
            DefoldApiReferenceArchive apiRefArchive = await DownloadDefoldApiRefArchive(release);

            var annotationsOutputDirectory = $"defold-lua-{release.Version}";
            var defoldyOutputDirectory = $"defoldy-{release.Version}";

            try {
                GenerateAnnotations(apiRefArchive, annotationsOutputDirectory);
                GenerateHelperLuaModules(apiRefArchive, defoldyOutputDirectory);

                ZipFile.CreateFromDirectory(annotationsOutputDirectory, $"./{annotationsOutputDirectory}.zip");
                ZipFile.CreateFromDirectory(defoldyOutputDirectory, $"./{defoldyOutputDirectory}.zip");
            } finally {
                Directory.Delete(annotationsOutputDirectory, true);
                Directory.Delete(defoldyOutputDirectory, true);
            }
        }

        static void GenerateHelperLuaModules(DefoldApiReferenceArchive apiRefArchive, string outputDirectory)
        {
            SaveFile(outputDirectory, "defoldy_hashes.lua", GenerateHashesForIncomingMessages(apiRefArchive));
            SaveFile(outputDirectory, "defoldy_msgs.lua", GenerateFunctionsForOutgoingMessages(apiRefArchive));
            SaveFile(outputDirectory, "defoldy.lua", new[] {
                "local M = require(\"defoldy_msgs\")",
                "M.h = require(\"defoldy_hashes\")",
                "return M",
            });
        }

        static void GenerateAnnotations(DefoldApiReferenceArchive apiRefArchive, string outputDirectory)
        {
            SaveFile(outputDirectory, "_readme.txt", new[] {
                $"Annotations for Defold API v{apiRefArchive.Release.Version} ({apiRefArchive.Release.Type.ToString("G").ToLower()}) - {apiRefArchive.Release.Sha1}",
                "Generated with https://github.com/mikatuo/Defold-Lua-Annotations under the MIT license"
            });
            SaveFile(outputDirectory, "base_defold.lua", GenerateDefoldBaseTypesAnnotations());
            foreach (var filename in apiRefArchive.Files) {
                RawApiReference apiRef = apiRefArchive.ExtractAndDeserialize(filename);
                // clean filenames
                var destFilenameWithoutExtension = apiRef.Info.Name
                    .Replace(" ", "_").Replace("-", "")
                    .ToLower();
                SaveFile(outputDirectory, $"{destFilenameWithoutExtension}.lua", GenerateLuaAnnotations(apiRef));
            }
        }

        #region Private Methods
        static async Task<DefoldRelease> FindDefoldRelease(ReleaseType type, string version)
        {
            var releases = await new ListDefoldReleases().DownloadAsync();
            var releasesWithDocs = releases.Where(x => x.ReferenceDocsArchiveUrl != null);

            if (StringComparer.OrdinalIgnoreCase.Equals(version, "latest"))
                return releasesWithDocs.OrderByDescending(x => x.Version).First(x => x.Type == type);

            var release = releasesWithDocs.SingleOrDefault(x => x.Version == version && x.Type == type);
            if (release == null)
                throw new Exception($"Can not find a {type.ToString("G").ToLowerInvariant()} release with the version {version}");
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

        static void SaveFile(string outputDirectory, string filename, IEnumerable<string> lines)
        {
            Directory.CreateDirectory(outputDirectory);
            var path = Path.Combine(outputDirectory, filename);
            using (var f = new StreamWriter(path)) {
                foreach (var line in lines)
                    f.WriteLine(line);
            }
        }
        #endregion
    }
}