using App.Dtos;
using App.Utils;
using System.Text.RegularExpressions;

namespace App.Parsers
{
    class DefoldReturnValueParser : Parser<RawApiRefReturnValue, DefoldReturnValue>
    {
        RawApiRefReturnValue ReturnValue => Input;

        public DefoldReturnValueParser(RawApiRefReturnValue input)
            : base(input) { }

        public override DefoldReturnValue Parse()
        {
            return new DefoldReturnValue {
                Name = ReturnValue.Name.StripHtmlMarkup(),
                Description = ParseDescription(),
                Types = ParseTypes(),
            };
        }

        public static DefoldReturnValue Parse(RawApiRefReturnValue input)
        {
            var parser = new DefoldReturnValueParser(input);
            return parser.Parse();
        }

        string ParseDescription()
        {
            if (ParameterIsTable())
                return ParseTableDescription();

            return ReturnValue.Description.StripHtmlMarkup().Replace("\n", " ");
        }

        string[] ParseTypes()
        {
            if (ParameterIsTable())
                return new[] { ParseTableType(ReturnValue.Description) };

            return ReturnValue.Types;
        }

        #region Parse table type
        bool ParameterIsTable()
            => ReturnValue.Types.Length == 1 && ReturnValue.Types[0] == "table";

        string ParseTableDescription()
        {
            var regex = new Regex(@"<dt>(?<description>[^<:]+):</dt>");
            var match = regex.Match(ReturnValue.Description);
            if (match.Success)
                return match.Groups["description"].Value;
            return ParseDescriptionUntilLineBreak() ?? ReturnValue.Description.StripHtmlMarkup();
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
        string ParseDescriptionUntilLineBreak()
        {
            var newLineIndex = ReturnValue.Description.IndexOf("\n");
            return newLineIndex > 0
                ? ReturnValue.Description[..newLineIndex].TrimEnd(':').StripHtmlMarkup()
                : ReturnValue.Description.StripHtmlMarkup();
        }
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
