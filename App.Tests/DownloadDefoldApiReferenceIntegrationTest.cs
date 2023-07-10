using FluentAssertions;

namespace App.Tests
{
    [TestFixture]
    [Category("Integration")]
    public class DownloadDefoldApiReferenceIntegrationTest
    {
        DownloadDefoldApiReferenceArchive Sut;

        [SetUp]
        public void Setup()
        {
            Sut = new DownloadDefoldApiReferenceArchive();
        }

        [Test]
        public async Task DownloadAsync_can_download_and_parse_docs_from_defold_website()
        {
            var release = new DefoldRelease {
                Version = "1.4.7",
                Sha1 = "",
                Type = ReleaseType.Stable,
                ReferenceDocsArchiveUrl = "https://github.com/defold/defold/releases/download/1.4.7/ref-doc.zip",
            };

            DefoldApiReferenceArchive apiReference = await Sut.DownloadAsync(release);

            apiReference.Should().BeEquivalentTo(ExpectedApiReference(release));
        }

        #region Test Helpers
        DefoldApiReferenceArchive ExpectedApiReference(DefoldRelease release)
        {
            return new DefoldApiReferenceArchive {
                Release = release,
                Files = new [] {
                    "base_doc.json",
                    "buffer_doc.json",
                    "builtins_doc.json",
                    "collectionfactory_doc.json",
                    "collectionproxy_doc.json",
                    "crash_doc.json",
                    "factory_doc.json",
                    "go_doc.json",
                    "gui_doc.json",
                    "liveupdate_doc.json",
                    "html5_doc.json",
                    "http_doc.json",
                    "image_doc.json",
                    "json_doc.json",
                    "label_doc.json",
                    "model_doc.json",
                    "msg_doc.json",
                    "particlefx_doc.json",
                    "physics_doc.json",
                    "profiler_doc.json",
                    "render_doc.json",
                    "resource_doc.json",
                    "socket_doc.json",
                    "sound_doc.json",
                    "sprite_doc.json",
                    "sys_doc.json",
                    "tilemap_doc.json",
                    "timer_doc.json",
                    "vmath_doc.json",
                    "window_doc.json",
                    "zlib_doc.json",
                }
            };
        }
        #endregion
    }
}
