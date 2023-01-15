using App.Infrastructure;
using Core;

namespace App.Annotators
{
    class FunctionAnnotator : Annotator
    {
        bool ReturnsNothing => Element.ReturnValues == null || Element.ReturnValues.Length == 0;

        public FunctionAnnotator(ApiRefElement functionElement)
            : base(functionElement)
        {
        }

        public override IEnumerable<string> GenerateAnnotations()
        {
            if (SkipConflictingElements())
                return Result;

            Append(DescriptionAnnotation());
            Append(ParametersAnnotation());
            // Some functions in Defold API reference docs have the same name, but different parameters
            // in example: msg.post(), msg.post(urlstring), msg.post(socket, path, fragment).
            // To make intellisense work in the IntelliJ IDEA correctly we have to skip 
            // msg.post() and msg.post(urlstring) elements and add them as overloads.
            Append(CustomFunctionOverloadsOrEmpty());
            Append(FunctionOverloadsAnnotationOrEmpty());
            Append(ReturnsAnnotationOrEmpty());
            Append(FunctionDefinition());

            return Result;
        }

        #region Private Methods
        static Dictionary<string, Predicate<ApiRefElement>> _skipConflictingElements = new Dictionary<string, Predicate<ApiRefElement>> {
            // skip msg.url() and msg.url(urlstring)
            ["msg.url"] = apiRef => apiRef.Parameters.Length is 0 or 1,
        };
        bool SkipConflictingElements()
        {
            if (_skipConflictingElements.ContainsKey(Element.Name)) {
                var shouldBeSkipped = _skipConflictingElements[Element.Name](Element);
                return shouldBeSkipped;
            }
            return false;
        }

        IEnumerable<string> DescriptionAnnotation()
            => Element.Description.Split("\n")
                .Select(x => x.Trim())
                .Select(x => $"---{x}");

        IEnumerable<string> ParametersAnnotation()
            => Element.Parameters.Select(x => x.ToAnnotation());

        static Dictionary<string, string[]> _customOverloadAnnotations = new Dictionary<string, string[]> {
            // add skipped overloads into the main 'msg.url' function annotation
            ["msg.url"] = new string[] {
                "---@overload fun(urlstring: string): url",
            },
        };
        IEnumerable<string> CustomFunctionOverloadsOrEmpty()
        {
            if (!_customOverloadAnnotations.ContainsKey(Element.Name))
                yield break;

            foreach (string customOverloadAnnotation in _customOverloadAnnotations[Element.Name])
                yield return customOverloadAnnotation;
        }

        IEnumerable<string> FunctionOverloadsAnnotationOrEmpty()
        {
            if (Element.Parameters.All(x => x.Required))
                yield break;

            var parameters = new List<ApiRefParameter>(Element.Parameters.Length);
            parameters.AddRange(Element.Parameters.Where(x => x.Required));
            int requiredParametersCount = parameters.Count;
            parameters.AddRange(Element.Parameters.Where(x => x.Optional).SkipLast(1));

            for (int i = parameters.Count; i >= requiredParametersCount; i--)
                yield return FunctionOverloadAnnotation(parameters.Take(i));
        }

        static Dictionary<string, string> _customReturnAnnotations = new Dictionary<string, string> {
            // TODO: submit a PR for Defold headers with the returned types?
            // in example https://github.com/defold/defold/blob/1ae302ec33d4514408c04ad3ae5d3c1efe2057bd/engine/lua/src/lua_os.doc_h
            ["os.time"] = "---@return number",
            ["math.random"] = "---@return number",
        };
        IEnumerable<string> ReturnsAnnotationOrEmpty()
        {
            if (ReturnsNothing) {
                if (_customReturnAnnotations.ContainsKey(Element.Name))
                    yield return _customReturnAnnotations[Element.Name];
                yield break;
            }
            foreach (ApiRefReturnValue returnValue in Element.ReturnValues)
                yield return $"---@return {returnValue.Types.JoinToString("|")} {returnValue.Description}";
        }

        string FunctionDefinition()
            => $"function {Element.Name}({Element.Parameters.Select(x => x.Name).JoinToString(", ")}) end";

        string FunctionOverloadAnnotation(IEnumerable<ApiRefParameter> parameters)
        {
            var formattedParams = parameters.Select(x => $"{x.Name}: {x.TypeAnnotation()}");
            if (ReturnsNothing) {
                return $"---@overload fun({formattedParams.JoinToString(", ")})";
            } else {
                var returnTypes = Element.ReturnValues.Select(x => x.ToAnnitation());
                var formattedReturnTypes = returnTypes.JoinToString(", ");
                return $"---@overload fun({formattedParams.JoinToString(", ")}): {formattedReturnTypes}";
            }
        }
        #endregion
    }
}
