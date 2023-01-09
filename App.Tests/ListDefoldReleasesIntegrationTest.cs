using FluentAssertions;

namespace App.Tests
{
    [TestFixture]
    [Category("Integration")]
    public class ListDefoldReleasesIntegrationTest
    {
        ListDefoldReleases Sut;

        [Test]
        public async Task DownloadAsync_downloads_page_returns_some_stable_releases()
        {
            Sut = new ListDefoldReleases(ReleaseType.Stable);

            var releases = await Sut.DownloadAsync();

            releases.Should().AllSatisfy(release => {
                release.Version.Should().MatchRegex(@"\d+\.\d+\.\d+");
                release.Sha1.Should().MatchRegex(@"[a-z0-9]{40}");
                release.Type.Should().Be(ReleaseType.Stable);
            });
        }

        [Test]
        public async Task DownloadAsync_downloads_page_returns_some_alpha_releases()
        {
            Sut = new ListDefoldReleases(ReleaseType.Alpha);

            var releases = await Sut.DownloadAsync();

            releases.Should().AllSatisfy(release => {
                release.Version.Should().MatchRegex(@"\d+\.\d+\.\d+-alpha");
                release.Sha1.Should().MatchRegex(@"[a-z0-9]{40}");
                release.Type.Should().Be(ReleaseType.Alpha);
            });
        }
    }
}