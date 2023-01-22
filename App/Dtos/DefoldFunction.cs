namespace App.Dtos
{
    public class DefoldFunction
    {
        public string Name { get; set; }
        public string Brief { get; set; }
        public string[] Description { get; set; }
        public DefoldParameter[] Parameters { get; set; }
        public DefoldReturnValue[] ReturnValues { get; set; }
        public string Examples { get; set; }
        public DefoldFunctionOverload[] Overloads { get; set; }

        public DefoldFunction()
        {
            Description = Array.Empty<string>();
            Parameters = Array.Empty<DefoldParameter>();
            ReturnValues = Array.Empty<DefoldReturnValue>();
            Overloads = Array.Empty<DefoldFunctionOverload>();
        }
    }
}
