using FluentAssertions;
using Octokit;

namespace App.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class ListDefoldReleasesTest
    {
        StubbedListDefoldReleases Sut;

        [SetUp]
        public void Setup()
        {
            Sut = new StubbedListDefoldReleases();
        }

        [Test]
        public async Task DownloadAsync_when_there_are_github_releases_returns_correctly_parsed_releases()
        {
            Sut.LoadReleasesFromGithubReturns = new List<Release> {
                GithubRelease("v1.4.8 - beta", prerelease: false, ReleaseAsset("ref-doc.zip", "https://github.com/defold/defold/releases/download/1.4.8-beta/ref-doc.zip")),
                GithubRelease("v1.4.8 - alpha", prerelease: true, ReleaseAsset("ref-doc.zip", "https://github.com/defold/defold/releases/download/1.4.8-alpha/ref-doc.zip")),
                GithubRelease("v1.4.7 - stable", prerelease: false, ReleaseAsset("ref-doc.zip", "https://github.com/defold/defold/releases/download/1.4.7/ref-doc.zip")),
                GithubRelease("v1.2.171", prerelease: false),
            };

            var releases = await Sut.DownloadAsync();

            releases.Should().BeEquivalentTo(new[] {
                new DefoldRelease { Version = "1.4.8", Sha1 = "", Type = ReleaseType.Beta, ReferenceDocsArchiveUrl = "https://github.com/defold/defold/releases/download/1.4.8-beta/ref-doc.zip" },
                new DefoldRelease { Version = "1.4.8", Sha1 = "", Type = ReleaseType.Alpha, ReferenceDocsArchiveUrl = "https://github.com/defold/defold/releases/download/1.4.8-alpha/ref-doc.zip" },
                new DefoldRelease { Version = "1.4.7", Sha1 = "", Type = ReleaseType.Stable, ReferenceDocsArchiveUrl = "https://github.com/defold/defold/releases/download/1.4.7/ref-doc.zip" },
                new DefoldRelease { Version = "1.2.171", Sha1 = "", Type = ReleaseType.Unknown, ReferenceDocsArchiveUrl = null },
            });
        }

        [Test]
        public async Task DownloadAsync_when_there_are_no_github_releases_returns_empty_result()
        {
            Sut.LoadReleasesFromGithubReturns = new List<Release>();

            var releases = await Sut.DownloadAsync();

            releases.Should().BeEquivalentTo(Array.Empty<DefoldRelease>());
        }

        #region Test Helpers
        static Release GithubRelease(string name, bool prerelease, params ReleaseAsset[] assets)
        {
            return new Release("", "", "", "", 0, "", "", "", name, "", false, prerelease, DateTime.Now, null, null, "", "", assets);
        }
        
        static ReleaseAsset ReleaseAsset(string name, string url)
        {
            return new ReleaseAsset("", 0, "", name, "", "", "", 0, 0, DateTime.Now, DateTime.Now, url, null);
        }

        class StubbedListDefoldReleases : ListDefoldReleases
        {
            public List<Release> LoadReleasesFromGithubReturns = new List<Release>();

            public StubbedListDefoldReleases()
                : base() {}

            protected internal override Task<IReadOnlyList<Release>> LoadReleasesFromGithub()
            {
                return Task.FromResult(LoadReleasesFromGithubReturns.AsReadOnly() as IReadOnlyList<Release>);
            }
        }
        #endregion
    }

}