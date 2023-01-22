using App.Dtos;
using App.Utils;

namespace App.Annotators
{
    class MessageAnnotator : Annotator<DefoldMessage>
    {
        public MessageAnnotator(DefoldMessage element)
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
            => Element.Description.Select(x => $"---{x}");

        IEnumerable<string> ParametersAnnotations()
            => Element.Parameters.Select(x => $"---@field {x.Name} {TypeAnnotation(x)} {x.Description}");
        
        string TypeAnnotation(DefoldParameter parameter)
        {
            var typesAnnotation = parameter.Types.JoinToString("|");
            if (parameter.Optional && !parameter.Types.Contains("nil"))
                typesAnnotation += "|nil";
            return typesAnnotation.Length == 0
                ? "any"
                : typesAnnotation;
        }
        #endregion
    }
}
