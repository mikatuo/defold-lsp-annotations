using App.Dtos;
using App.Utils;

namespace App.Parsers
{
    class DefoldFunctionParser : Parser<RawApiRefElement, DefoldFunction>
    {
        RawApiRefElement Element => Input;

        public DefoldFunctionParser(RawApiRefElement input)
            : base(input) { }

        public override DefoldFunction Parse()
        {
            var parameters = Element.Parameters.Select(DefoldParameterParser.Parse).ToArray();
            var returnValues = Element.ReturnValues.Select(ParseReturnValue).ToArray();

            return new DefoldFunction {
                Name = Element.Name.StripHtmlMarkup(),
                Brief = Element.Brief.StripHtmlMarkup(),
                Description = ParseDescription(),
                Parameters = parameters,
                ReturnValues = returnValues,
                Overloads = OverloadsFromOptionalParams(parameters, returnValues).ToArray(),
                // TODO: parse examples
                Examples = "TODO",
            };
        }

        public static DefoldFunction Parse(RawApiRefElement input)
        {
            var parser = new DefoldFunctionParser(input);
            return parser.Parse();
        }

        #region Private Methods
        string[] ParseDescription()
            => Element.Description.StripHtmlMarkup().Split("\n").Select(x => x.Trim()).ToArray();

        DefoldReturnValue ParseReturnValue(RawApiRefReturnValue raw)
        {
            var parser = new DefoldReturnValueParser(raw);
            return parser.Parse();
        }

        IEnumerable<DefoldFunctionOverload> OverloadsFromOptionalParams(DefoldParameter[] parameters, DefoldReturnValue[] returnValues)
        {
            if (parameters.All(x => x.Required))
                yield break;

            var @params = new List<DefoldParameter>(parameters);

            while (@params.Exists(x => x.Optional)) {
                @params.RemoveAt(@params.Count - 1); // remove last parameter
                yield return new DefoldFunctionOverload {
                    Parameters = @params.ToArray(),
                    ReturnValues = returnValues,
                };
            }
        }
        #endregion
    }
}
