using App.Annotators;
using App.Dtos;

namespace App.AnnotatorsTeal
{
    class VariableAnnotator : Annotator<RawApiRefElement>
    {
        public VariableAnnotator(RawApiRefElement variableElement)
            : base(variableElement)
        {
        }

        public override IEnumerable<string> GenerateAnnotations()
        {
            var variableName = Element.Name.Split(".").Last();
            var descriptions = DescriptionAnnotation();
            var description = descriptions.Count() > 0 ? " ---" + string.Join(" ", descriptions) : string.Empty;

            Append($"\t{variableName}: constant{description}");

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
