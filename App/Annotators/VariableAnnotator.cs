using App.Dtos;

namespace App.Annotators
{
    class VariableAnnotator : Annotator<RawApiRefElement>
    {
        public VariableAnnotator(RawApiRefElement variableElement)
            : base(variableElement)
        {
        }

        public override IEnumerable<string> GenerateAnnotations()
        {
            Append(DescriptionAnnotation());
            Append($"{Element.Name} = nil");

            return Result;
        }

        IEnumerable<string> DescriptionAnnotation()
            => Element.Description.Split("\n")
                .Select(x => x.Trim())
                .Select(x => $"---{x}");
    }
}
