using RestSharp;

namespace App
{
    public class DownloadDefoldApiReferenceArchive
    {
        public async Task<DefoldApiReferenceArchive> DownloadAsync(DefoldRelease release)
        {
            byte[]? apiReferenceZip = await DownloadApiReferenceZip(release);
            return new DefoldApiReferenceArchive(release, apiReferenceZip);
        }

        #region Private Methods
        protected virtual async Task<byte[]?> DownloadApiReferenceZip(DefoldRelease release)
        {
            var client = new RestClient();
            var request = new RestRequest(release.RefDocUrl(), Method.Get);
            var response = await client.GetAsync(request);
            return response.RawBytes;
        }
        #endregion
    }
}
