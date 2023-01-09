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
        public async Task DownloadAsync_with_stubbed_downloaded_json_docs_correctly_parses_api_reference()
        {
            var release = new DefoldRelease { Version = "1.4.1", Sha1 = "8f96e450ddfb006a99aa134fdd373cace3760571", Type = ReleaseType.Stable };

            DefoldApiReferenceArchive apiReference = await Sut.DownloadAsync(release);

            apiReference.Should().BeEquivalentTo(ExpectedApiReference());
        }

        #region Test Helpers
        class StubbedDownloadDefoldApiReference : DownloadDefoldApiReferenceArchive
        {
            public byte[] DownloadApiReferenceZipReturns;

            protected override Task<byte[]> DownloadApiReferenceZip(DefoldRelease release)
            {
                return Task.FromResult(DownloadApiReferenceZipReturns);
            }
        }

        DefoldApiReferenceArchive ExpectedApiReference()
        {
            return new DefoldApiReferenceArchive {
                Files = new [] {
                    "base_doc.json",
                    "bit_doc.json",
                    "buffer_doc.json",
                    "builtins_doc.json",
                    "camera_doc.json",
                    "collectionfactory_doc.json",
                    "collectionproxy_doc.json",
                    "coroutine_doc.json",
                    "crash_doc.json",
                    "debug_doc.json",
                    "engine_doc.json",
                    "facebook_doc.json",
                    "factory_doc.json",
                    "go_doc.json",
                    "gui_doc.json",
                    "html5_doc.json",
                    "http_doc.json",
                    "iac_doc.json",
                    "iap_doc.json",
                    "image_doc.json",
                    "io_doc.json",
                    "json_doc.json",
                    "label_doc.json",
                    "math_doc.json",
                    "model_doc.json",
                    "msg_doc.json",
                    "os_doc.json",
                    "package_doc.json",
                    "particlefx_doc.json",
                    "physics_doc.json",
                    "profiler_doc.json",
                    "push_doc.json",
                    "render_doc.json",
                    "resource_doc.json",
                    "sharedlibrary_doc.json",
                    "socket_doc.json",
                    "sound_doc.json",
                    "sprite_doc.json",
                    "string_doc.json",
                    "sys_doc.json",
                    "table_doc.json",
                    "tilemap_doc.json",
                    "timer_doc.json",
                    "vmath_doc.json",
                    "webview_doc.json",
                    "window_doc.json",
                    "zlib_doc.json",
                }
            };
        }
        #endregion
    }
}
