using App.Dtos;
using App.Utils;

namespace App.Annotators
{
    class FunctionAnnotator : Annotator<DefoldFunction>
    {
        public FunctionAnnotator(DefoldFunction functionElement)
            : base(functionElement)
        {
        }

        public override IEnumerable<string> GenerateAnnotations()
        {
            Append(DescriptionAnnotation());
            Append(ParametersAnnotation());
            Append(FunctionOverloadsAnnotationOrEmpty());
            Append(ReturnsAnnotationOrEmpty());
            Append(FunctionDefinition());

            return Result;
        }

        #region Private Methods
        IEnumerable<string> DescriptionAnnotation()
            => Element.Description.Select(x => $"---{x}");

        IEnumerable<string> ParametersAnnotation()
            => Element.Parameters.Select(x => $"---@param {x.Name} {TypeAnnotation(x)} {x.Description}");

        string TypeAnnotation(DefoldParameter parameter)
        {
            var typesAnnotation = parameter.Types.JoinToString("|");
            if (parameter.Optional && !parameter.Types.Contains("nil"))
                typesAnnotation += "|nil";
            return typesAnnotation.Length == 0
                ? "any"
                : typesAnnotation;
        }

        string TypeAnnotation(DefoldReturnValue returnValue)
        {
            var typesAnnotation = returnValue.Types.JoinToString("|");
            return typesAnnotation.Length == 0
                ? "any"
                : typesAnnotation;
        }
        
        IEnumerable<string> FunctionOverloadsAnnotationOrEmpty()
        {
            foreach (var overload in Element.Overloads) {
                var formattedParams = overload.Parameters.Select(x => $"{x.Name}: {TypeAnnotation(x)}");
                if (overload.ReturnValues.Length == 0) {
                    yield return $"---@overload fun({formattedParams.JoinToString(", ")})";
                } else {
                    var returnTypes = Element.ReturnValues.Select(TypeAnnotation);
                    var formattedReturnTypes = returnTypes.JoinToString(", ");
                    yield return $"---@overload fun({formattedParams.JoinToString(", ")}): {formattedReturnTypes}";
                }
            }
        }

        static Dictionary<string, string> _customReturnAnnotations = new Dictionary<string, string> {
            // TODO: submit a PR for Defold headers with the returned types?
            // in example https://github.com/defold/defold/blob/1ae302ec33d4514408c04ad3ae5d3c1efe2057bd/engine/lua/src/lua_os.doc_h
            ["os.time"] = "---@return number",
            ["math.random"] = "---@return number",
        };
        IEnumerable<string> ReturnsAnnotationOrEmpty()
        {
            if (Element.ReturnValues.Length == 0) {
                if (_customReturnAnnotations.ContainsKey(Element.Name))
                    yield return _customReturnAnnotations[Element.Name];
                yield break;
            }
            foreach (var returnValue in Element.ReturnValues)
                yield return $"---@return {returnValue.Types.JoinToString("|")} {returnValue.Name} {returnValue.Description}";
        }

        string FunctionDefinition()
            => $"function {Element.Name}({Element.Parameters.Select(x => x.Name).JoinToString(", ")}) end";
        #endregion
    }
}
