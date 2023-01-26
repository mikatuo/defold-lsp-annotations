using App.Tests.Properties;
using FluentAssertions;

namespace App.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class DownloadDefoldApiReferenceTest
    {
        StubbedDownloadDefoldApiReference Sut;

        [SetUp]
        public void Setup()
        {
            Sut = new StubbedDownloadDefoldApiReference();
        }

        [Test]
        public async Task DownloadAsync_with_stubbed_downloaded_json_docs_correctly_parses_api_reference()
        {
            Sut.DownloadApiReferenceZipReturns = Resources.ref_doc_1_4_1_zip;
            var release = new DefoldRelease { Version = "1.4.1", Sha1 = "8f96e450ddfb006a99aa134fdd373cace3760571", Type = ReleaseType.Stable };

            DefoldApiReferenceArchive apiRefArchive = await Sut.DownloadAsync(release);

            apiRefArchive.Should().BeEquivalentTo(new DefoldApiReferenceArchive {
                Release = release,
                Files = ExpectedFiles(),
            });
        }

        #region Test Helpers
        class StubbedDownloadDefoldApiReference : DownloadDefoldApiReferenceArchive
        {
            public byte[]? DownloadApiReferenceZipReturns;

            protected override Task<byte[]?> DownloadApiReferenceZip(DefoldRelease release)
            {
                return Task.FromResult(DownloadApiReferenceZipReturns);
            }
        }

        string[] ExpectedFiles()
        {
            return new [] {
                "base_doc.json",
                "buffer_doc.json",
                "builtins_doc.json",
                "collectionfactory_doc.json",
                "collectionproxy_doc.json",
                "crash_doc.json",
                "factory_doc.json",
                "go_doc.json",
                "gui_doc.json",
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
            };
        }
        #endregion
    }
}
