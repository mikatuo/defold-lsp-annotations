using App;

namespace ConsoleApp.Extensions
{
    class ProgramArgs
    {
        public string ReleaseVersion { get; } = "latest";
        public ReleaseType ReleaseType { get; set; } = ReleaseType.Stable;

        public ProgramArgs(string[] args)
        {
            switch (args.Length) {
                case 1:
                    ReleaseVersion = args[0];
                    break;
                case 2:
                    ReleaseVersion = args[0];
                    if (Enum.TryParse<ReleaseType>(args[1], ignoreCase: true, result: out var releaseType))
                        ReleaseType = releaseType;
                    break;
            }
        }
    }
}
