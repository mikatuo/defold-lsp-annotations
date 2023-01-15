namespace App.Annotators
{
    interface IAnnotator
    {
        IEnumerable<string> GenerateAnnotations();
    }
}