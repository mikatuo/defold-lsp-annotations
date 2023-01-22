namespace App.Dtos
{
    public class DefoldMessage
    {
        public string Name { get; set; }
        public string Brief { get; set; }
        public string[] Description { get; set; }
        public DefoldParameter[] Parameters { get; set; }
        public bool Outgoing { get; set; }
        public bool Incoming { get; set; }

        public DefoldMessage()
        {
            Description = Array.Empty<string>();
            Parameters = Array.Empty<DefoldParameter>();
        }
    }
}
