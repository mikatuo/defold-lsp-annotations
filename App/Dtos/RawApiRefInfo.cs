namespace App.Dtos
{
    public class RawApiRefInfo
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Brief { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string File { get; set; }
        public string Group { get; set; }

        public IEnumerable<string> DescriptionAnnotation()
            => Description.Split("\n")
                .Select(x => x.Trim())
                .Select(x => $"---{x}");
    }
}
