using App;
using ConsoleApp.Extensions;
using Core;

namespace ConsoleApp
{
    internal class Program
    {
        static string OutputDirectory = ".defold";

        static async Task Main(string[] args)
        {
            var options = new ProgramArgs(args);

            DefoldRelease release = await FindDefoldRelease(options.ReleaseType, options.ReleaseVersion);
            DefoldApiReferenceArchive apiRefArchive = await DownloadDefoldApiRefArchive(release);

            SaveFile("_readme.txt", new [] {
                $"Annotations for Defold API v{release.Version} ({release.Type.ToString("G").ToLower()})",
            });
            SaveFile("base_defold.lua", GenerateDefoldBaseTypesAnnotations());
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
            // TODO: get Sha1 of the latest Defold release by a different HTTP call
            if (StringComparer.OrdinalIgnoreCase.Equals(version, "latest"))
                return releases.OrderByDescending(x => x.Version).First();

            var release = releases.SingleOrDefault(x => x.Version == version);
            if (release == null)
                throw new Exception($"Can not find a release with the version {version}");
            return release;
        }

        static async Task<DefoldApiReferenceArchive> DownloadDefoldApiRefArchive(DefoldRelease release)
            => await new DownloadDefoldApiReferenceArchive().DownloadAsync(release);

        static IEnumerable<string> GenerateDefoldBaseTypesAnnotations()
            => new GenerateLuaAnnotations().DefoldBaseAnnotations();

        static IEnumerable<string> GenerateLuaAnnotations(DefoldApiReference apiRef)
            => new GenerateLuaAnnotations().ForApiReference(apiRef);

        static void SaveFile(string filename, IEnumerable<string> lines)
        {
            Directory.CreateDirectory(OutputDirectory);
            using (var f = new StreamWriter($"./{OutputDirectory}/{filename}")) {
                foreach (var line in lines)
                    f.WriteLine(line);
            }
        }
        #endregion
    }
}