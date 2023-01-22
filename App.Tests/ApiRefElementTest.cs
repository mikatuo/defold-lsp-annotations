using App.Dtos;
using FluentAssertions;

namespace App.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class ApiRefElementTest
    {
        RawApiRefElement Sut;

        [SetUp]
        public void Setup()
        {
            Sut = new RawApiRefElement();
        }
        

        [Test]
        public void ToAnnotation_given_a_variable_returns_correct_annotation()
        {
            Sut.Type = "VARIABLE";
            Sut.Name = "go.PLAYBACK_LOOP_BACKWARD";
            Sut.Brief = "loop backward";
            Sut.Description = "loop backward";
            Sut.Parameters = Array.Empty<RawApiRefParameter>();
            Sut.ReturnValues = Array.Empty<RawApiRefReturnValue>();

            var annotations = Sut.ToAnnotation();

            Assert.That(annotations, Is.EqualTo(
                "---loop backward\n" +
                "go.PLAYBACK_LOOP_BACKWARD = nil"
            ));
        }
    }
}
