using System.Text;

namespace App.Tests.TestData
{
    static class Extensions
    {
        public static byte[] ReadFileAsBytes(this string relativePath)
        {
            var testFile = Path.Combine(TestContext.CurrentContext.TestDirectory, relativePath);
            return File.ReadAllBytes(testFile);
        }

        public static string ReadFileAsString(this string relativePath)
            => Encoding.UTF8.GetString(relativePath.ReadFileAsBytes());
    }
}
