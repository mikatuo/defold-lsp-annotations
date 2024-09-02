using App.Annotators;
using App.Dtos;

namespace App.AnnotatorsTeal
{
    class VariableTealAnnotator : Annotator<RawApiRefElement>
    {
        public VariableTealAnnotator(RawApiRefElement variableElement)
            : base(variableElement)
        {
        }

        public override IEnumerable<string> GenerateAnnotations()
        {
            var variableName = Element.Name.Split(".").Last();
            var descriptions = DescriptionAnnotation();
            var description = descriptions.Count() > 0 ? " ---" + string.Join(" ", descriptions) : string.Empty;

            switch (variableName) {
                case "RENDER_TARGET_DEFAULT":
                    Append($"\t{variableName}: render_target{description}");
                    break;
                default:
                    Append($"\t{variableName}: constant{description}");
                    break;
            }


            return Result;
        }

        IEnumerable<string> DescriptionAnnotation()
            => Element.Description
                .Trim()
                .Split("\n")
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x));
    }
}
