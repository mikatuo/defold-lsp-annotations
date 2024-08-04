using App.Annotators;
using App.Dtos;
using App.Utils;

namespace App.AnnotatorsTeal
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
            Append($"global record {Element.Name}_msg");
            Append(ParametersAnnotations());
            Append($"end");

            return Result;
        }

        #region Private Methods
        IEnumerable<string> DescriptionAnnotation()
            => Element.Description.Select(x => $"---{x}");

        IEnumerable<string> ParametersAnnotations()
            => Element.Parameters.Select(x => $"\t{x.Name}: {TypeAnnotation(x)} --- {x.Description}");

        string TypeAnnotation(DefoldParameter parameter)
        {
            var typesAnnotation = parameter.Types
                .Select(t => GenerateTealAnnotations.IdentifierRenameMap.TryGetValue(t, out var typeReplacement) ? typeReplacement : t)
                .JoinToString("|");

            if (parameter.Optional && !parameter.Types.Contains("nil"))
                typesAnnotation += "|nil";
            return typesAnnotation.Length == 0
                ? "any"
                : typesAnnotation;
        }
        #endregion
    }
}
