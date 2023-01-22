namespace App.Annotators
{
    abstract class Annotator<T> : IAnnotator
    {
        protected T Element { get; }
        protected List<string> Result { get; } = new List<string>();

        public Annotator(T element)
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