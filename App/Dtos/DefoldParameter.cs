namespace App.Dtos
{
    public class DefoldParameter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Types { get; set; }
        public bool Optional { get; set; }
        public bool Required => !Optional;
    }
}