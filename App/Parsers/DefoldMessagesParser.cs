using App.Dtos;

namespace App.Parsers
{
    class DefoldMessagesParser : Parser<RawApiRefElement[], DefoldMessage[]>
    {
        RawApiRefElement[] Msgs => Input;

        public DefoldMessagesParser(RawApiRefElement[] input)
            : base(input) { }

        public override DefoldMessage[] Parse()
        {
            return Msgs.Select(DefoldMessageParser.Parse)
                .ToArray();
        }

        public static DefoldMessage[] Parse(IEnumerable<RawApiRefElement> input)
        {
            var parser = new DefoldMessagesParser(input.ToArray());
            return parser.Parse();
        }
    }
}
