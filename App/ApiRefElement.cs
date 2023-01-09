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
        bool ReturnsNothing => ReturnValues == null || ReturnValues.Length == 0;

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
            if (SkipConflictingElements())
                return null;

            switch (Type.ToLowerInvariant()) {
                case "function":
                    return FunctionAnnotation();
                case "variable":
                    return VariableAnnotation();
                default:
                    return null;
            }
        }

        #region Private Methods
        string? FunctionAnnotation()
        {
            var lines = new List<string>();

            lines.AddRange(DescriptionAnnotation());
            lines.AddRange(ParametersAnnotation());
            // Some functions in Defold API reference docs have the same name, but different parameters
            // in example: msg.post(), msg.post(urlstring), msg.post(socket, path, fragment).
            // To make it work in the IntelliJ IDEA correctly we have to skip 
            // msg.post() and msg.post(urlstring) elements and we have to add them as overloads here.
            lines.AddRange(CustomFunctionOverloadsOrEmpty());
            lines.AddRange(FunctionOverloadsAnnotationOrEmpty());
            lines.AddRange(ReturnsAnnotationOrEmpty());
            lines.Add(FunctionDefinition());

            return string.Join("\n", lines);
        }

        string? VariableAnnotation()
        {
            var lines = new List<string>();

            lines.AddRange(DescriptionAnnotation());
            lines.Add($"{Name} = nil");

            return string.Join("\n", lines);
        }

        bool SkipConflictingElements()
        {
            if (_skipConflictingElements.ContainsKey(Name)) {
                var shouldBeSkipped = _skipConflictingElements[Name](this);
                return shouldBeSkipped;
            }
            return false;
        }

        IEnumerable<string> DescriptionAnnotation()
            => Description.Split("\n")
                .Select(x => x.Trim())
                .Select(x => $"---{x}");

        IEnumerable<string> ParametersAnnotation()
            => Parameters.Select(x => x.ToAnnotation());

        static Dictionary<string, string> _customReturnAnnotations = new Dictionary<string, string> {
            // TODO: submit a PR for Defold headers with the returned types?
            // in example https://github.com/defold/defold/blob/1ae302ec33d4514408c04ad3ae5d3c1efe2057bd/engine/lua/src/lua_os.doc_h
            [ "os.time" ] = "---@return number",
            [ "math.random" ] = "---@return number",
        };
        IEnumerable<string> ReturnsAnnotationOrEmpty()
        {
            if (ReturnsNothing) {
                if (_customReturnAnnotations.ContainsKey(Name))
                    yield return _customReturnAnnotations[Name];
                yield break;
            }
            foreach (ApiRefReturnValue returnValue in ReturnValues)
                yield return $"---@return {string.Join("|", returnValue.Types)} {returnValue.Description}";
        }
        
        static Dictionary<string, Predicate<ApiRefElement>> _skipConflictingElements = new Dictionary<string, Predicate<ApiRefElement>> {
            // skip msg.url() and msg.url(urlstring)
            [ "msg.url" ] = apiRef => apiRef.Parameters.Length is 0 or 1,
        };
        static Dictionary<string, string[]> _customOverloadAnnotations = new Dictionary<string, string[]> {
            // add skipped overloads into the main 'msg.url' function annotation
            [ "msg.url" ] = new string[] {
                "---@overload fun(urlstring: string): url",
            },
        };
        IEnumerable<string> CustomFunctionOverloadsOrEmpty()
        {
            if (!_customOverloadAnnotations.ContainsKey(Name))
                yield break;

            foreach (string customOverloadAnnotation in _customOverloadAnnotations[Name])
                yield return customOverloadAnnotation;
        }

        string FunctionDefinition()
            => $"function {Name}({string.Join(", ", Parameters.Select(x => x.Name))}) end";
        
        IEnumerable<string> FunctionOverloadsAnnotationOrEmpty()
        {
            if (Parameters.All(x => x.Required))
                yield break;

            var parameters = new List<ApiRefParameter>(Parameters.Length);
            parameters.AddRange(Parameters.Where(x => x.Required));
            int requiredParametersCount = parameters.Count;
            parameters.AddRange(Parameters.Where(x => x.Optional).SkipLast(1));

            for (int i = parameters.Count; i >= requiredParametersCount; i--)
                yield return FunctionOverloadAnnotation(parameters.Take(i));
        }

        string FunctionOverloadAnnotation(IEnumerable<ApiRefParameter> parameters)
        {
            var formattedParams = parameters.Select(x => $"{x.Name}: {x.TypeAnnotation()}");
            if (ReturnsNothing) {
                return $"---@overload fun({string.Join(", ", formattedParams)})";
            } else {
                var returnTypes = ReturnValues.Select(x => x.ToAnnitation());
                var formattedReturnTypes = string.Join(", ", returnTypes);
                return $"---@overload fun({string.Join(", ", formattedParams)}): {formattedReturnTypes}";
            }
        }
        #endregion
    }
}
