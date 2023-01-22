namespace App.Dtos
{
    public class DefoldApi
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Brief { get; set; }
        public string Description { get; set; }

        public DefoldFunction[] Functions { get; set; }
        public DefoldMessage[] Messages { get; set; }
    }
}
