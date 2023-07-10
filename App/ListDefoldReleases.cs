using Octokit;

namespace App
{
    public class ListDefoldReleases
    {
        GitHubClient _github;

        public ListDefoldReleases()
        {
            _github = new GitHubClient(new ProductHeaderValue("MikaDefoldLuaAnnotations"));
        }

        public async Task<IList<DefoldRelease>> DownloadAsync()
        {
            IReadOnlyList<Release> releases = await LoadReleasesFromGithub();
            return releases.Select(githubRelease => ParseDefoldRelease(githubRelease))
                .Where(ReleaseWasParsedCorrectly())
                .ToArray();
        }

        #region Private Methods
        protected internal virtual async Task<IReadOnlyList<Release>> LoadReleasesFromGithub()
            => await _github.Repository.Release.GetAll("defold", "defold");

        Func<DefoldRelease, bool> ReleaseWasParsedCorrectly()
            => defoldRelease => defoldRelease != null;

        DefoldRelease ParseDefoldRelease(Release release)
        {
            var (Version, Type) = ExtractVersionAndType(release);
            return new DefoldRelease(Version, Type) {
                // older releases do not have ref-doc.zip assets so it is not always available
                ReferenceDocsArchiveUrl = GetReferenceDocsDownloadUrlOrDefault(release),
            };
        }

        (string Version, ReleaseType Type) ExtractVersionAndType(Release release)
        {
            var tokens = release.Name.Split('-').Select(x => x.Trim()).ToArray();
            var version = tokens[0].TrimStart('v');
            var type = tokens.Length > 1
                ? Enum.Parse<ReleaseType>(tokens[1], ignoreCase: true)
                : ReleaseType.Unknown;
            return (version, type);
        }

        static string? GetReferenceDocsDownloadUrlOrDefault(Release release)
            => release.Assets.FirstOrDefault(x => x.Name == "ref-doc.zip")?.BrowserDownloadUrl;
        #endregion
    }
}