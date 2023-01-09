using App.Tests.Properties;
using FluentAssertions;

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
            Sut = new StubbedListDefoldReleases(ReleaseType.Stable);
        }

        [Test]
        public async Task DownloadAsync_with_stubbed_defold_downloads_page_returns_correct_releases()
        {
            Sut.LoadDefoldDownloadsPageReturns = Resources.d_defold_com_stable_html;

            var releases = await Sut.DownloadAsync();

            releases.Should().BeEquivalentTo(new [] {
                new DefoldRelease { Version = "1.4.1", Sha1 = "8f96e450ddfb006a99aa134fdd373cace3760571", Type = ReleaseType.Stable },
                new DefoldRelease { Version = "1.4.0", Sha1 = "9c44c4a9b6cbc9d0cb66b7027b7c984bf364a568", Type = ReleaseType.Stable },
                new DefoldRelease { Version = "1.3.7", Sha1 = "f0ad06a2f1fbf0e9cbddbf96162a75bc006d84bb", Type = ReleaseType.Stable },
                new DefoldRelease { Version = "1.3.6", Sha1 = "905234d8da2e642f1075c73aaa1bfb72e49199e3", Type = ReleaseType.Stable },
                new DefoldRelease { Version = "1.3.5", Sha1 = "28eafea5a8bfedfddc621a7cd00b39f25bd34922", Type = ReleaseType.Stable },
                new DefoldRelease { Version = "1.3.4", Sha1 = "80b1b73fd9cdbd4682c2583403fddfbaf0919107", Type = ReleaseType.Stable },
                new DefoldRelease { Version = "1.3.3", Sha1 = "c2ab1630e34f1311d8340d81494cf5317d25fe16", Type = ReleaseType.Stable },
                new DefoldRelease { Version = "1.3.2", Sha1 = "287c945fab310c324493e08b191ee1b1538ef973", Type = ReleaseType.Stable },
                new DefoldRelease { Version = "1.3.1", Sha1 = "06bc078e490fd7d94ec01e38abac989f6cc351a5", Type = ReleaseType.Stable },
                new DefoldRelease { Version = "1.3.0", Sha1 = "0e77ba11ac957ee01878bbde2e6ac0c9fae6dc01", Type = ReleaseType.Stable },
            });
        }

        [Test]
        public async Task DownloadAsync_when_stubbed_downloads_page_is_null_returns_empty_result()
        {
            Sut.LoadDefoldDownloadsPageReturns = null;

            var releases = await Sut.DownloadAsync();

            releases.Should().BeEquivalentTo(Array.Empty<DefoldRelease>());
        }

        #region Test Helpers
        class StubbedListDefoldReleases : ListDefoldReleases
        {
            public string? LoadDefoldDownloadsPageReturns;

            public StubbedListDefoldReleases(ReleaseType releaseType)
                : base(releaseType) {}

            protected internal override Task<string?> LoadDefoldDownloadsPage()
            {
                return Task.FromResult(LoadDefoldDownloadsPageReturns);
            }
        }
        #endregion
    }

}