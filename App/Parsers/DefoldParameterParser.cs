using App.Dtos;
using App.Utils;
using System.Text.RegularExpressions;

namespace App.Parsers
{
    class DefoldParameterParser : Parser<RawApiRefParameter, DefoldParameter>
    {
        RawApiRefParameter Param => Input;

        public DefoldParameterParser(RawApiRefParameter input)
            : base(input) { }

        public override DefoldParameter Parse()
        {
            var optional = Param.Name.StartsWith('[');

            return new DefoldParameter {
                Name = Param.Name.Trim('[', ']'),
                Description = ParseDescription().Replace("\n", " "),
                Types = ParseTypes(),
                Optional = optional,
            };
        }

        public static DefoldParameter Parse(RawApiRefParameter input)
        {
            var parser = new DefoldParameterParser(input);
            return parser.Parse();
        }

        string ParseDescription()
        {
            if (ParameterIsTable())
                return ParseTableDescription();
            if (ParameterIsCallbackFunction())
                return ParseCallbackFunctionDescription();

            return Param.Description.StripHtmlMarkup();
        }

        string[] ParseTypes()
        {
            if (ParameterIsTable())
                return new[] { ParseTableType(Param.Description) };
            if (ParameterIsCallbackFunction())
                return new[] { ParseCallbackFunctionType(Param.Description) };

            return Param.Types;
        }

        #region Parse table type
        bool ParameterIsTable()
            => Param.Types.Length == 1 && Param.Types[0] == "table";

        string ParseTableDescription()
        {
            var regex = new Regex(@"<dt>(?<description>[^<:]+):</dt>");
            var match = regex.Match(Param.Description);
            if (match.Success)
                return match.Groups["description"].Value;
            // a fix for [play_properties] in
            // https://defold.com/ref/stable/sprite/?q=sprite.play_flipbook#sprite.play_flipbook
            return ParseCallbackFunctionDescription() ?? Param.Description.StripHtmlMarkup();
        }

        string ParseTableType(string description)
        {
            var tableParams = ExtractParamsFromDescription(description);
            if (tableParams.Count == 0)
                return "table"; // either HTML markup has changed or there were no parameters in the description
            var formattedParams = tableParams.Select(x => $"{x.name}:{x.type}").JoinToString(", ");
            return "{" + formattedParams + "}";
        }
        #endregion

        #region Parse callback function type
        bool ParameterIsCallbackFunction()
            => Param.Types.Length == 1 && Param.Types[0].StartsWith("function(");

        string ParseCallbackFunctionDescription()
        {
            var newLineIndex = Param.Description.IndexOf("\n");
            return newLineIndex > 0
                ? Param.Description[..newLineIndex].TrimEnd(':').StripHtmlMarkup()
                : Param.Description.StripHtmlMarkup();
        }

        string ParseCallbackFunctionType(string description)
        {
            var functionParams = GetCallbackFunctionParameterNames();
            if (functionParams.Length == 0)
                return "fun()";

            var paramsFromDescription = ExtractParamsFromDescription(description);
            string formattedParams = GenerateFunctionParamsWithTypes(functionParams, paramsFromDescription);
            return $"fun({formattedParams})";
        }

        static string GenerateFunctionParamsWithTypes(string[] functionParams, List<(string name, string type)> paramsFromDescription)
        {
            return functionParams.Select(name => {
                var extractedType = paramsFromDescription.FirstOrDefault(x => name.EqualsIgnoreCase(x.name)).type;
                return extractedType != null
                    ? $"{name}:{extractedType}"
                    : name;
            }).JoinToString(", ");
        }

        string[] GetCallbackFunctionParameterNames()
            => new Regex(@"(?:function)|\(|\)|\s").Replace(Param.Types[0], "").Split(",", StringSplitOptions.RemoveEmptyEntries);
        #endregion

        #region Extract parameters from description
        static List<(string name, string type)> ExtractParamsFromDescription(string description)
        {
            var parameters = new List<(string name, string type)>();

            var descriptionLines = description.Split("\n");
            for (int i = 0; i < descriptionLines.Length - 1; i++) {
                string line = descriptionLines[i];

                // example: https://defold.com/ref/stable/sound/#sound.play
                // [play_properties] or [complete_function] parameters
                string? paramName = ExtractParamName(line);
                string? paramType = ExtractParamType(descriptionLines, ref i);

                if (paramName == null || paramType == null)
                    continue;

                // example: https://defold.com/ref/stable/sound/#sound.play
                // [complete_function] parameter
                if (paramType == "table") {
                    var nestedParameters = ExtractNestedParameters(descriptionLines, ref i);
                    if (nestedParameters != null) {
                        var nestedTableType = nestedParameters.Select(x => $"{x.name}:{x.type}").JoinToString(", ");
                        paramType = "{" + nestedTableType + "}";
                    }
                }
                parameters.Add((paramName, paramType));
            }

            if (parameters.Count == 0) {
                // fix for [options] in
                // https://defold.com/ref/stable/http/?q=http.request#http.request
                var i = 0;
                var nestedParameters = ExtractNestedParameters(descriptionLines, ref i);
                if (nestedParameters != null)
                    return nestedParameters;
            }

            return parameters;
        }

        static string? ExtractParamName(string line)
        {
            var nameRegex = new Regex(@"<dt>\s*<code>(?<name>[^<]+)</code>");
            var nameMatch = nameRegex.Match(line);
            if (nameMatch.Success)
                return nameMatch.Groups["name"].Value;
            return null;
        }

        static string? ExtractParamType(string[] descriptionLines, ref int i)
        {
            var typeRegex = new Regex(@"<span class=""type"">(?<type>[^<]+)</span>");

            var j = i + 1;
            for (; j < descriptionLines.Length - 1; j++) {
                var line = descriptionLines[j];
                if (line.Contains("<dt>"))
                    return null; // reached next parameter, no type was found
                var typeMatch = typeRegex.Match(line);
                if (typeMatch.Success) {
                    i = j;
                    return typeMatch.Groups["type"].Value;
                }
            }
            return null; // failure
        }

        static List<(string name, string type)>? ExtractNestedParameters(string[] descriptionLines, ref int i)
        {
            var parameters = new List<(string name, string type)>();
            var nestedParamRegex = new Regex(@"<span class=""type"">(?<type>[^<]+)</span>\s*<code>(?<name>[^<]+)</code>");

            var j = i + 1;
            for (; j < descriptionLines.Length - 1; j++) {
                var line = descriptionLines[j + 1];
                if (line.Contains("<dl>") || line.Contains("<dt>")) {
                    // either we didn't find any parameters or we got them all
                    break;
                }
                if (line.Contains("<li>")) {
                    // found a parameter
                    var paramMatch = nestedParamRegex.Match(line);
                    if (paramMatch.Success)
                        parameters.Add((
                            name: paramMatch.Groups["name"].Value,
                            type: paramMatch.Groups["type"].Value
                        ));
                }
            }

            if (parameters.Count != 0) {
                i = j;
                return parameters;
            } else {
                return null; // failure
            }
        }
        #endregion
    }
}
