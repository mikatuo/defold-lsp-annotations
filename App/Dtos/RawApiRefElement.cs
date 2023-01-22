using App.Annotators;
using App.Utils;

namespace App.Dtos
{
    public class RawApiRefElement
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Brief { get; set; }
        public string Description { get; set; }
        public RawApiRefReturnValue[] ReturnValues { get; set; }
        public RawApiRefParameter[] Parameters { get; set; }
        public string Examples { get; set; }
        public string Replaces { get; set; }
        public string Error { get; set; }
        public string[] TParams { get; set; }
        public string[] Members { get; set; }
        public string[] Notes { get; set; }
        public bool OutgoingMessage { get; set; }
        public bool IncomingMessage { get; set; }

        public RawApiRefElement()
        {
            Error = "";
            Members = Array.Empty<string>();
            Notes = Array.Empty<string>();
            Parameters = Array.Empty<RawApiRefParameter>();
            Replaces = "";
            TParams = Array.Empty<string>();
        }

        public string? ToAnnotation()
        {
            string? result = null;
            switch (Type.ToLowerInvariant())
            {
                case "variable":
                    result = new VariableAnnotator(this).GenerateAnnotations().JoinToString("\n");
                    break;
            }
            return string.IsNullOrWhiteSpace(result) ? null : result;
        }
    }
}
