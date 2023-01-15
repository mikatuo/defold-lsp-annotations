using Core;

namespace App.Annotators
{
    class MessageAnnotator : Annotator
    {
        public MessageAnnotator(ApiRefElement element)
            : base(element)
        {
        }

        public override IEnumerable<string> GenerateAnnotations()
        {
            Append(DescriptionAnnotation());
            Append($"---@class {Element.Name}_msg");
            Append(ParametersAnnotations());

            return Result;
        }

        #region Private Methods
        IEnumerable<string> DescriptionAnnotation()
            => Element.Description.Split("\n")
                .Select(x => x.Trim())
                .Select(x => $"---{x}");

        IEnumerable<string> ParametersAnnotations()
        {
            foreach (var parameter in Element.Parameters) {
                var types = parameter.TypeAnnotation();
                yield return $"---@field {parameter.Name} {types}";
            }
        }
        #endregion
    }
}
