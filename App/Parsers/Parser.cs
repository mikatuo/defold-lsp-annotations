namespace App.Parsers
{
    abstract class Parser<TInput, TResult>
    {
        protected TInput Input { get; }

        public Parser(TInput input)
        {
            Input = input;
        }

        public abstract TResult Parse();
    }
}