using App.Annotators;
using App.Dtos;
using App.Utils;

namespace App.AnnotatorsTeal
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
            Append(ReturnsAnnotationOrEmpty());
            Append(FunctionDefinitionSplitter(Element.Parameters, Element.ReturnValues));
            Append(FunctionOverloadsAnnotationOrEmpty());

            return Result;
        }

        #region Private Methods
        IEnumerable<string> DescriptionAnnotation()
            => Element.Description.Select(x => $"\t---{x}");

        IEnumerable<string> ParametersAnnotation()
            => Element.Parameters.Select(x => $"\t---{x.Name} {TypeAnnotation(x)} {x.Description}");

        string TypeAnnotation(DefoldParameter parameter)
            => TypeAnnotation(parameter.Types, parameter.Optional);


        string TypeAnnotation(DefoldReturnValue returnValue)
            => TypeAnnotation(returnValue.Types);

        string TypeAnnotation(string[] types, bool optional = false)
        {
            var typesAnnotation = types.Select(type => {
                if (type.StartsWith("fun(")) {
                    type = $"function({type.Substring(4)}";
                }

                if (GenerateTealAnnotations.IdentifierRenameMap.TryGetValue(type, out var newType)) {
                    type = newType;
                }

                return type;
            }).JoinToString("|");

            if (optional && !types.Contains("nil"))
                typesAnnotation += "|nil";
            return typesAnnotation.Length == 0
                ? "any"
                : typesAnnotation;
        }

        string ParameterName(string name)
        {
            if (GenerateTealAnnotations.IdentifierRenameMap.TryGetValue(name, out var value)) {
                return value;
            }

            return name;
        }

        IEnumerable<string> FunctionOverloadsAnnotationOrEmpty()
        {
            foreach (var overload in Element.Overloads) {
                foreach (var annotations in FunctionDefinitionSplitter(overload.Parameters, overload.ReturnValues)) {
                    yield return annotations;
                }
            }
        }

        // static Dictionary<string, string> _customReturnAnnotations = new Dictionary<string, string>
        // {
        //     // TODO: submit a PR for Defold headers with the returned types?
        //     // in example https://github.com/defold/defold/blob/1ae302ec33d4514408c04ad3ae5d3c1efe2057bd/engine/lua/src/lua_os.doc_h
        //     ["os.time"] = "\t---@return number",
        //     ["math.random"] = "\t---@return number",
        // };
        IEnumerable<string> ReturnsAnnotationOrEmpty()
        {
            // if (Element.ReturnValues.Length == 0)
            // {
            //     if (_customReturnAnnotations.ContainsKey(Element.Name))
            //         yield return _customReturnAnnotations[Element.Name];
            //     yield break;
            // }
            foreach (var returnValue in Element.ReturnValues)
                yield return $"\t---{returnValue.Name} {returnValue.Description}";
        }

        string FunctionDefinition(IEnumerable<DefoldParameter> parameters, IEnumerable<DefoldReturnValue> returnValues)
        {
            var functionName = Element.Name.Split(".").Last();
            var formattedParams = parameters.Select(x => $"{ParameterName(x.Name)}: {TypeAnnotation(x)}");

            if (returnValues.Count() == 0) {
                return $"\t{functionName}: function({formattedParams.JoinToString(", ")})";
            } else {
                var returnTypes = returnValues.Select(TypeAnnotation);
                var formattedReturnTypes = returnTypes.JoinToString(", ");
                return $"\t{functionName}: function({formattedParams.JoinToString(", ")}): {formattedReturnTypes}";
            }
        }
        #endregion

        #region Hack to split functions that have conflicting union parameters

        // TODO: submit a PR for Defold to split these out in the api definition
        static string[] SplitTypes = new string[] { "vector3", "vector4", "quaternion", "quat" };
        IEnumerable<string> FunctionDefinitionSplitter(IEnumerable<DefoldParameter> parameters, IEnumerable<DefoldReturnValue> returnValues)
        {
            var detailed = GetSplitDefinition(parameters, returnValues);
            var splitTypes = detailed.SplitTypes;
            var detailedParameters = detailed.Parameters;
            var detailedReturnValues = detailed.ReturnValues;
            var nonSplit = detailedParameters.All(p => !p.IsVariable) && detailedReturnValues.All(r => !r.IsVariable);

            if (nonSplit) {
                yield return FunctionDefinition(parameters, returnValues);
                yield break;
            }

            var hasSignatureWithoutSplitTypes = parameters.All(p => p.Types.Any(t => !splitTypes.Contains(t))) && returnValues.All(r => r.Types.Any(t => !splitTypes.Contains(t)));

            if (hasSignatureWithoutSplitTypes) {
                var newParametersWithoutSplit = parameters.Select(p => new DefoldParameter {
                    Name = p.Name,
                    Description = p.Description,
                    Optional = p.Optional,
                    Types = p.Types.Where(t => !splitTypes.Contains(t)).ToArray()
                });

                var newReturnValuesWithoutSplit = returnValues.Select(r => new DefoldReturnValue {
                    Name = r.Name,
                    Description = r.Description,
                    Types = r.Types.Where(t => !splitTypes.Contains(t)).ToArray()
                });

                yield return FunctionDefinition(newParametersWithoutSplit, newReturnValuesWithoutSplit);
            }

            foreach (var splitType in splitTypes) {
                var splitExistsInAllVariable = detailedParameters.Where(p => p.IsVariable).All(p => p.Parameter.Types.Contains(splitType))
                    && detailedReturnValues.Where(r => r.IsVariable).All(r => r.ReturnValue.Types.Contains(splitType));

                if (!splitExistsInAllVariable) {
                    continue;
                }

                var splitTypeArray = new string[] { splitType };

                var newParametersWithSplitReplacement = detailedParameters.Select(p => new DefoldParameter {
                    Name = p.Parameter.Name,
                    Description = p.Parameter.Description,
                    Optional = p.Parameter.Optional,
                    Types = !p.IsVariable ? p.Parameter.Types : splitTypeArray
                });

                var newReturnValuesWithSplitReplacement = detailedReturnValues.Select(r => new DefoldReturnValue {
                    Name = r.ReturnValue.Name,
                    Description = r.ReturnValue.Description,
                    Types = !r.IsVariable ? r.ReturnValue.Types : splitTypeArray
                });

                yield return FunctionDefinition(newParametersWithSplitReplacement, newReturnValuesWithSplitReplacement);
            }
        }

        bool IsVariableType(string[] splitTypes, string[] types)
        {
            var splitTypeCount = types.Count(t => splitTypes.Contains(t));
            var normalTypeCount = types.Length - splitTypeCount;

            return splitTypeCount > 1 || (normalTypeCount > 0 && splitTypeCount > 0);
        }

        private SplitDefinition GetSplitDefinition(IEnumerable<DefoldParameter> parameters, IEnumerable<DefoldReturnValue> returnValues)
        {
            var splitTypes = SplitTypes;

            switch (Element.Name) {
                case "go.delete":
                    splitTypes = new string[] { "table" };
                    break;
                case "go.property":
                    splitTypes = splitTypes.Concat(new string[] { "resource" }).ToArray();
                    break;
                case "render.set_camera":
                    splitTypes = new string[] { "handle" };
                    break;
            }

            return new SplitDefinition {
                SplitTypes = splitTypes,
                Parameters = parameters.Select(p => new SplitDefinitionParameter { Parameter = p, IsVariable = IsVariableType(splitTypes, p.Types) }),
                ReturnValues = returnValues.Select(r => new SplitDefinitionReturnValue { ReturnValue = r, IsVariable = IsVariableType(splitTypes, r.Types) }),
            };
        }

        private class SplitDefinition
        {
            public string[] SplitTypes { get; set; }
            public IEnumerable<SplitDefinitionParameter> Parameters { get; set; }
            public IEnumerable<SplitDefinitionReturnValue> ReturnValues { get; set; }
        }

        private class SplitDefinitionParameter
        {
            public bool IsVariable { get; init; }
            public DefoldParameter Parameter { get; init; }
        }

        private class SplitDefinitionReturnValue
        {
            public bool IsVariable { get; init; }
            public DefoldReturnValue ReturnValue { get; init; }
        }

        #endregion
    }
}

