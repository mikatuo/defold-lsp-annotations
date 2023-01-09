using RestSharp;
using System.Text.RegularExpressions;

namespace App
{
    public class ListDefoldReleases
    {
        RestClient _client;
        ReleaseType _releaseType;

        public ListDefoldReleases(ReleaseType type)
        {
            _releaseType = type;
            _client = new RestClient();
        }

        public async Task<IList<DefoldRelease>> DownloadAsync()
        {
            string? downloadsPageHtml = await LoadDefoldDownloadsPage();
            return ExtractReleasesFromHtml(downloadsPageHtml);
        }

        #region Private Methods
        protected internal virtual async Task<string?> LoadDefoldDownloadsPage()
        {
            var request = new RestRequest($"https://d.defold.com/{_releaseType.ToString("G").ToLower()}/", Method.Get);
            RestResponse response = await _client.GetAsync(request);
            return response.Content;
        }

        IList<DefoldRelease> ExtractReleasesFromHtml(string? downloadsPageHtml)
        {
            if (downloadsPageHtml == null)
                return Array.Empty<DefoldRelease>();

            var regex = new Regex(@"""tag"":\s*""([^""]*)"",\s*""sha1"":\s*""([^""]*)""");
            return regex.Matches(downloadsPageHtml).Select(match => {
                var version = match.Groups[1].Value;
                var sha1 = match.Groups[2].Value;
                return new DefoldRelease(version, sha1) {
                    Type = _releaseType,
                };
            }).ToArray();
        }
        #endregion
    }
}