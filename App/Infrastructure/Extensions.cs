namespace App.Infrastructure
{
    static class Extensions
    {
        public static string JoinToString(this IEnumerable<string> collection, string delimiter)
            => string.Join(delimiter, collection);
    }
}
