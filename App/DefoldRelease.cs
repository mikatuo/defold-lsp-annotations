namespace App
{
    public class DefoldRelease
    {
        public string Version { get; internal set; }
        public string Sha1 { get; internal set; }
        public ReleaseType Type { get; set; }

        internal DefoldRelease() { } // for unit tests
        public DefoldRelease(string version, string sha1)
        {
            Version = version;
            Sha1 = sha1;
        }

        public string RefDocUrl()
            => $"https://d.defold.com/archive/{Type.ToString("G").ToLower()}/{Sha1}/engine/share/ref-doc.zip";
    }
}