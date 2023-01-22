using App.Dtos;

namespace App.Parsers
{
    class DefoldFunctionsParser : Parser<RawApiRefElement[], DefoldFunction[]>
    {
        RawApiRefElement[] Elements => Input;

        public DefoldFunctionsParser(RawApiRefElement[] input)
            : base(input) { }

        public override DefoldFunction[] Parse()
        {
            var functions = Elements.Select(DefoldFunctionParser.Parse).ToArray();
            return MergeFunctionsWithSameNames(functions).ToArray();
        }

        public static DefoldFunction[] Parse(IEnumerable<RawApiRefElement> input)
        {
            var parser = new DefoldFunctionsParser(input.ToArray());
            return parser.Parse();
        }

        IEnumerable<DefoldFunction> MergeFunctionsWithSameNames(IEnumerable<DefoldFunction> functions)
        {
            foreach (var group in functions.GroupBy(x => x.Name)) {
                DefoldFunction[] functionsWithSameName = group.ToArray();

                // main function will be the one with most parameters
                var mainFunction = functionsWithSameName.OrderByDescending(x => x.Parameters.Length).First();
                var otherFunctions = functionsWithSameName.Except(new[] { mainFunction });

                mainFunction.Overloads = mainFunction.Overloads
                    .Concat(otherFunctions.Select(AsOverload)) // add other functions as overloads
                    .Concat(otherFunctions.SelectMany(x => x.Overloads)) // and their overloads too
                    .Distinct(DefoldFunctionOverloadUniquenessEqualityComparer.Instance) // eliminate duplicates
                    .OrderBy(x => x.Parameters.Length)
                    .ToArray();

                yield return mainFunction;
            }
        }

        DefoldFunctionOverload AsOverload(DefoldFunction function)
            => new DefoldFunctionOverload {
                Parameters = function.Parameters,
                ReturnValues = function.ReturnValues,
            };
    }
}
