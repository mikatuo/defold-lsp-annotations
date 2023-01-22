using App.Dtos;
using App.Utils;

namespace App.Parsers
{
    class DefoldDocsParser : Parser<RawApiReference, DefoldApi>
    {
        RawApiReference Api => Input;

        public DefoldDocsParser(RawApiReference input)
            : base(input) {}

        public override DefoldApi Parse()
        {
            return new DefoldApi {
                Namespace = Api.Info.Namespace.StripHtmlMarkup(),
                Name = Api.Info.Name.StripHtmlMarkup(),
                Brief = Api.Info.Brief.StripHtmlMarkup(),
                Description = Api.Info.Description.StripHtmlMarkup(),
                Functions = DefoldFunctionsParser.Parse(Api.Elements.Where(type: "function")),
                Messages = DefoldMessagesParser.Parse(Api.Elements.Where(type: "message")),
                // TODO: parse variables
            };
        }
    }
}
