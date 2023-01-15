using Core;

namespace App.Annotators
{
    abstract class Annotator : IAnnotator
    {
        protected ApiRefElement Element { get; }
        protected List<string> Result { get; } = new List<string>();

        public Annotator(ApiRefElement element)
        {
            Element = element;
        }
        
        public abstract IEnumerable<string> GenerateAnnotations();

        protected void Append(string line)
            => Result.Add(line);

        protected void Append(IEnumerable<string> lines)
            => Result.AddRange(lines);
    }
}