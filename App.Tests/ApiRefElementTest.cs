using Core;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Security.Cryptography;

namespace App.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class ApiRefElementTest
    {
        ApiRefElement Sut;

        [SetUp]
        public void Setup()
        {
            Sut = new ApiRefElement();
        }
        

        [Test]
        public void ToAnnotation_given_a_variable_returns_correct_annotation()
        {
            Sut.Type = "VARIABLE";
            Sut.Name = "go.PLAYBACK_LOOP_BACKWARD";
            Sut.Brief = "loop backward";
            Sut.Description = "loop backward";
            Sut.Parameters = Array.Empty<ApiRefParameter>();
            Sut.ReturnValues = Array.Empty<ApiRefReturnValue>();

            var annotations = Sut.ToAnnotation();

            annotations.Should().Be(
                "---loop backward\n" +
                "go.PLAYBACK_LOOP_BACKWARD = nil");
        }

        [Test]
        public void ToAnnotation_given_a_function_element_with_only_required_parameters_returns_function_annotation_without_overload()
        {
            Sut.Type = "FUNCTION";
            Sut.Name = "msg.post";
            Sut.Brief = "posts a message to a receiving URL";
            Sut.Description = "Post a message to a receiving URL. The most common case is to send messages\nto a component. If the component part of the receiver is omitted, the message\nis broadcast to all components in the game object.\nThe following receiver shorthands are available:\n\n\".\" the current game object\n\"#\" the current component\n\n There is a 2 kilobyte limit to the message parameter table size.";
            Sut.Parameters = new [] {
                new ApiRefParameter {
                    Name = "receiver",
                    Description = "The receiver must be a string in URL-format, a URL object or a hashed string.",
                    Types = new [] { "string", "url", "hash" },
                },
                new ApiRefParameter {
                    Name = "message_id",
                    Description = "The id must be a string or a hashed string.",
                    Types = new [] { "string", "hash" },
                },
                new ApiRefParameter {
                    Name = "message",
                    Description = "a lua table with message parameters to send.",
                    Types = new [] { "table", "nil" },
                },
            };
            Sut.ReturnValues = new [] {
                new ApiRefReturnValue {
                    Name = "url",
                    Description = "a new URL",
                    Types = new [] { "url" },
                },
            };

            var annotations = Sut.ToAnnotation();

            annotations.Should().Be(
                "---Post a message to a receiving URL. The most common case is to send messages\n" +
                "---to a component. If the component part of the receiver is omitted, the message\n" +
                "---is broadcast to all components in the game object.\n" +
                "---The following receiver shorthands are available:\n" +
                "---\n" +
                "---\".\" the current game object\n" +
                "---\"#\" the current component\n" +
                "---\n" +
                "---There is a 2 kilobyte limit to the message parameter table size.\n" +
                "---@param receiver string|url|hash The receiver must be a string in URL-format, a URL object or a hashed string.\n" +
                "---@param message_id string|hash The id must be a string or a hashed string.\n" +
                "---@param message table|nil a lua table with message parameters to send.\n" +
                "---@return url a new URL\n" +
                "function msg.post(receiver, message_id, message) end");
        }

        [Test]
        public void ToAnnotation_given_a_function_element_with_two_required_parameters_returns_function_annotation_with_overloads()
        {
            Sut.Type = "FUNCTION";
            Sut.Name = "msg.something";
            Sut.Brief = "does somethings";
            Sut.Description = "First line of the description.\nSecond line of the description.";
            Sut.Parameters = new [] {
                new ApiRefParameter {
                    Name = "required_param1",
                    Description = "The first required parameter.",
                    Types = new [] { "string", "url", "hash" },
                },
                new ApiRefParameter {
                    Name = "optional_param1",
                    Optional = true,
                    Description = "The first optional parameter.",
                    Types = new [] { "string", "hash" },
                },
                new ApiRefParameter {
                    Name = "optional_param2",
                    Optional = true,
                    Description = "The second optional parameter.",
                    Types = new [] { "table", "nil" },
                },
            };

            var annotations = Sut.ToAnnotation();

            annotations.Should().Be(
                "---First line of the description.\n" +
                "---Second line of the description.\n" +
                "---@param required_param1 string|url|hash The first required parameter.\n" +
                "---@param optional_param1 string|hash The first optional parameter.\n" +
                "---@param optional_param2 table|nil The second optional parameter.\n" +
                "---@overload fun(required_param1: string|url|hash, optional_param1: string|hash)\n" +
                "---@overload fun(required_param1: string|url|hash)\n" +
                "function msg.something(required_param1, optional_param1, optional_param2) end"
            );
        }

        [Test]
        public void ToAnnotation_given_a_function_element_with_two_required_parameters_and_return_values_returns_function_annotation_with_two_overloads_with_returned_type()
        {
            Sut.Type = "FUNCTION";
            Sut.Name = "msg.something";
            Sut.Brief = "does somethings";
            Sut.Description = "First line of the description.\nSecond line of the description.";
            Sut.Parameters = new [] {
                new ApiRefParameter {
                    Name = "required_param1",
                    Description = "The first required parameter.",
                    Types = new [] { "string", "url", "hash" },
                },
                new ApiRefParameter {
                    Name = "optional_param1",
                    Optional = true,
                    Description = "The first optional parameter.",
                    Types = new [] { "string", "hash" },
                },
                new ApiRefParameter {
                    Name = "optional_param2",
                    Optional = true,
                    Description = "The second optional parameter.",
                    Types = new [] { "table", "nil" },
                },
            };
            Sut.ReturnValues = new [] {
                new ApiRefReturnValue {
                    Name = "url",
                    Description = "a new URL",
                    Types = new [] { "url", "string" },
                },
            };

            var annotations = Sut.ToAnnotation();

            annotations.Should().Be(
                "---First line of the description.\n" +
                "---Second line of the description.\n" +
                "---@param required_param1 string|url|hash The first required parameter.\n" +
                "---@param optional_param1 string|hash The first optional parameter.\n" +
                "---@param optional_param2 table|nil The second optional parameter.\n" +
                "---@overload fun(required_param1: string|url|hash, optional_param1: string|hash): url|string\n" +
                "---@overload fun(required_param1: string|url|hash): url|string\n" +
                "---@return url|string a new URL\n" +
                "function msg.something(required_param1, optional_param1, optional_param2) end"
            );
        }

        [Test]
        [TestCaseSource(nameof(SkippedElementTestCases))]
        public void ToAnnotation_given_an_element_that_should_be_skipped_as_conflicting_returns_null(string name, ApiRefParameter[] parameters)
        {
            Sut.Type = "FUNCTION";
            Sut.Name = name;
            Sut.Parameters = parameters;

            var annotations = Sut.ToAnnotation();

            annotations.Should().Be(null);
        }
        static object[] SkippedElementTestCases = {
            new object[] { "msg.url", NoParameters() },
            new object[] { "msg.url", Parameters("urlstring") },
        };

        #region TestHelpers
        static ApiRefParameter[] NoParameters()
            => Array.Empty<ApiRefParameter>();
        static ApiRefParameter[] Parameters(params string[] names)
            => names.Select(name => Parameter(name)).ToArray();
        static ApiRefParameter Parameter(string name)
            => new ApiRefParameter { Name = name };
        #endregion
    }
}
