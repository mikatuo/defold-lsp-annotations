using FluentAssertions;

namespace App.Tests
{
    [TestFixture]
    [Category("Integration")]
    public class ListDefoldReleasesIntegrationTest
    {
        ListDefoldReleases Sut;

        [Test]
        public async Task DownloadAsync_downloads_page_returns_some_releases()
        {
            Sut = new ListDefoldReleases();

            var releases = await Sut.DownloadAsync();

            var tenRecentReleases = releases.Take(10);
            tenRecentReleases.Should()
                .NotBeEmpty()
                .And.AllSatisfy(release => {
                    release.Version.Should().MatchRegex(@"\d+\.\d+\.\d+");
                    release.Sha1.Should().Be("");
                    release.Type.Should().NotBe(ReleaseType.Unknown);
                    release.ReferenceDocsArchiveUrl.Should().NotBeNullOrEmpty();
                });
        }
    }
}