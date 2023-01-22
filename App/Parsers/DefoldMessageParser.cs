using App.Dtos;
using App.Utils;

namespace App.Parsers
{
    class DefoldMessageParser : Parser<RawApiRefElement, DefoldMessage>
    {
        RawApiRefElement Msg => Input;

        public DefoldMessageParser(RawApiRefElement input)
            : base(input) { }

        public override DefoldMessage Parse()
        {
            var outgoing = IsOutgoing();

            return new DefoldMessage {
                Name = Msg.Name,
                Brief = Msg.Brief,
                Description = Msg.Description.StripHtmlMarkup().Split("\n"),
                Parameters = Msg.Parameters.Select(DefoldParameterParser.Parse).ToArray(),
                Outgoing = outgoing,
                Incoming = !outgoing,
            };
        }

        public static DefoldMessage Parse(RawApiRefElement input)
        {
            var parser = new DefoldMessageParser(input);
            return parser.Parse();
        }

        bool IsOutgoing()
        {
            // automatic identification does not work correctly for "animation_done"
            // because example text contains "msg.post" in it
            if (Msg.Name == "animation_done")
                return false;
            return Msg.Description.StartsWith("Post") || Msg.Examples.Contains("msg.post");
        }
    }
}
