using App.Dtos;
using FluentAssertions;

namespace App.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class ApiRefParameterTest
    {
        RawApiRefParameter Sut;

        [SetUp]
        public void Setup()
        {
            Sut = new RawApiRefParameter();
        }

        [Test]
        public void ToAnnotation_given_a_required_parameter_returns_valid_annotation()
        {
            Sut.Name = "receiver";
            Sut.Description = "The receiver must be a string in URL-format, a URL object or a hashed string.";
            Sut.Types = new[] { "string", "url", "hash" };

            string annotations = Sut.ToAnnotation();

            annotations.Should().Be("---@param receiver string|url|hash The receiver must be a string in URL-format, a URL object or a hashed string.");
        }

        [Test]
        public void ForParameter_given_an_optional_parameter_returns_valid_annotation()
        {
            Sut.Name = "message";
            Sut.Description = "a lua table with message parameters to send.";
            Sut.Types = new[] { "table", "nil" };
            Sut.Optional = true;

            string annotations = Sut.ToAnnotation();

            annotations.Should().Be("---@param message table|nil a lua table with message parameters to send.");
        }
    }
}
