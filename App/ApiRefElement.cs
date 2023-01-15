using App.Annotators;
using App.Infrastructure;

namespace Core
{
    public class ApiRefElement
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Brief { get; set; }
        public string Description { get; set; }
        public ApiRefReturnValue[] ReturnValues { get; set; }
        public ApiRefParameter[] Parameters { get; set; }
        public string Examples { get; set; }
        public string Replaces { get; set; }
        public string Error { get; set; }
        public string[] TParams { get; set; }
        public string[] Members { get; set; }
        public string[] Notes { get; set; }
        public bool OutgoingMessage { get; set; }
        public bool IncomingMessage { get; set; }

        public ApiRefElement()
        {
            Error = "";
            Members = Array.Empty<string>();
            Notes = Array.Empty<string>();
            Parameters = Array.Empty<ApiRefParameter>();
            Replaces = "";
            TParams = Array.Empty<string>();
        }

        public string? ToAnnotation()
        {
            string? result = null;
            switch (Type.ToLowerInvariant()) {
                case "function":
                    result = new FunctionAnnotator(this).GenerateAnnotations().JoinToString("\n");
                    break;
                case "variable":
                    result = new VariableAnnotator(this).GenerateAnnotations().JoinToString("\n");
                    break;
                case "message":
                    result = new MessageAnnotator(this).GenerateAnnotations().JoinToString("\n");
                    break;
            }
            return string.IsNullOrWhiteSpace(result) ? null : result;
        }
    }
}
