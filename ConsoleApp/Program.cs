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
            ///////////////////////////////////////////
            // uncomment a following line to generate /
            // annotations for a specific version     /
            ///////////////////////////////////////////
            // var options = new ProgramArgs(new[] { "1.6.0", "stable" });
            // var options = new ProgramArgs(new[] { "1.4.4-beta", "beta" })
            // var options = new ProgramArgs(new[] { "1.4.2-alpha", "alpha" });
            var options = new ProgramArgs(args);

            //////////////////////////////////////////////
            // uncomment the following lines to generate /
            // annotations from a local zip file         /
            //////////////////////////////////////////////
            // var release = new DefoldRelease("1.9.1", ReleaseType.Stable);
            // DefoldApiReferenceArchive apiRefArchive = await ParseDefoldApiFromFile("./bin/ref-doc.zip", release);

            DefoldRelease release = await FindDefoldRelease(options.ReleaseType, options.ReleaseVersion);
            DefoldApiReferenceArchive apiRefArchive = await DownloadDefoldApiRefArchive(release);

            var annotationsOutputDirectory = $"defold-lua-{release.Version}";
            var tealAnnotationsOutputDirectory = $"defold-teal-{release.Version}";
            var defoldyOutputDirectory = $"defoldy-{release.Version}";

            try {
                GenerateLuaAnnotations(apiRefArchive, annotationsOutputDirectory);
                GenerateTealAnnotations(apiRefArchive, tealAnnotationsOutputDirectory);
                GenerateHelperLuaModules(apiRefArchive, defoldyOutputDirectory);

                ZipFolder(annotationsOutputDirectory);
                ZipFolder(tealAnnotationsOutputDirectory);
                ZipFolder(defoldyOutputDirectory);
            } finally {
                Directory.Delete(annotationsOutputDirectory, true /* recursive */);
                Directory.Delete(tealAnnotationsOutputDirectory, true /* recursive */);
                Directory.Delete(defoldyOutputDirectory, true /* recursive */);
            }
        }

        static async Task<DefoldApiReferenceArchive> ParseDefoldApiFromFile(string apiRefArchiveFile, DefoldRelease release)
        {
            var apiReferenceZip = await File.ReadAllBytesAsync(apiRefArchiveFile);
            var apiRefArchive = new DefoldApiReferenceArchive(release, apiReferenceZip);
            return apiRefArchive;
        }

        static void GenerateLuaAnnotations(DefoldApiReferenceArchive apiRefArchive, string outputDirectory)
        {
            SaveFile(outputDirectory, "_readme.txt", new[] {
                $"Annotations for Defold API v{apiRefArchive.Release.Version} ({apiRefArchive.Release.Type.ToString("G").ToLower()}) - {apiRefArchive.Release.Sha1}",
                "Generated with https://github.com/mikatuo/Defold-Lua-Annotations under the MIT license"
            });
            SaveFile(outputDirectory, "base_defold.lua", GenerateDefoldBaseTypesLuaAnnotations());
            foreach (var filename in apiRefArchive.Files) {
                RawApiReference apiRef = apiRefArchive.ExtractAndDeserialize(filename);
                // clean filenames
                var destFilenameWithoutExtension = apiRef.Info.Name
                    .Replace(" ", "_")
                    .Replace("-", "")
                    .ToLower();
                SaveFile(outputDirectory, $"{destFilenameWithoutExtension}.lua", GenerateLuaAnnotations(apiRef));
            }
        }

        static void GenerateTealAnnotations(DefoldApiReferenceArchive apiRefArchive, string outputDirectory)
        {
            SaveFile(outputDirectory, "_readme.txt", new[] {
                $"Annotations for Defold API v{apiRefArchive.Release.Version} ({apiRefArchive.Release.Type.ToString("G").ToLower()}) - {apiRefArchive.Release.Sha1}",
                "Generated with https://github.com/mikatuo/Defold-Lua-Annotations under the MIT license"
            });
            var outputSeparateDir = Path.Join(outputDirectory, "separate");
            SaveFile(outputSeparateDir, "defold.d.tl", GenerateRootDefoldApiTealAnnotations());
            SaveFile(outputSeparateDir, "base_defold.d.tl", GenerateDefoldBaseTypesTealAnnotations());
            foreach (var filename in apiRefArchive.Files) {
                RawApiReference apiRef = apiRefArchive.ExtractAndDeserialize(filename);
                // clean filenames
                var destFilenameWithoutExtension = apiRef.Info.Name
                    .Replace(" ", "_")
                    .Replace(".", "_")
                    .Replace("-", "")
                    .ToLower();
                SaveFile(outputSeparateDir, $"{destFilenameWithoutExtension}.d.tl", GenerateTealAnnotations(apiRef));
            }

            var fullDefinition = App.GenerateTealAnnotations.GenerateFullDefinition(outputSeparateDir);

            SaveFile(outputDirectory, "defold.d.tl", fullDefinition);
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

        static void ZipFolder(string annotationsOutputDirectory)
        {
            ZipFile.CreateFromDirectory(annotationsOutputDirectory, $"./{annotationsOutputDirectory}.zip");
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

        static IEnumerable<string> GenerateDefoldBaseTypesLuaAnnotations()
            => new GenerateLuaAnnotations().DefoldBaseAnnotations();

        static IEnumerable<string> GenerateDefoldBaseTypesTealAnnotations()
            => new GenerateTealAnnotations().DefoldBaseAnnotations();

        static IEnumerable<string> GenerateRootDefoldApiTealAnnotations()
            => App.GenerateTealAnnotations.workingApi.Select(api => $"require(\"{api}\")");

        static IEnumerable<string> GenerateLuaAnnotations(RawApiReference apiRef)
            => new GenerateLuaAnnotations().ForApiReference(apiRef);

        static IEnumerable<string> GenerateTealAnnotations(RawApiReference apiRef)
            => new GenerateTealAnnotations().ForApiReference(apiRef);

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