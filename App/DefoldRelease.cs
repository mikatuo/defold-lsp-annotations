namespace App
{
    public class DefoldRelease
    {
        public string Version { get; internal set; }
        public string Sha1 { get; internal set; } = "";
        public ReleaseType Type { get; internal set; } = ReleaseType.Unknown;
        public string? ReferenceDocsArchiveUrl { get; internal set; }

        internal DefoldRelease() { } // for unit tests
        public DefoldRelease(string version, ReleaseType type)
        {
            Version = version;
            Type = type;
        }
    }
}