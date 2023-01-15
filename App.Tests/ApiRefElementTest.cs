using Core;
using FluentAssertions;

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

            Assert.That(annotations, Is.EqualTo(
                "---loop backward\n" +
                "go.PLAYBACK_LOOP_BACKWARD = nil"
            ));
        }

        [Test]
        public void ToAnnotation_given_a_message_returns_class_annotation()
        {
            Sut.Type = "MESSAGE";
            Sut.Name = "collision_response";
            Sut.Brief = "reports a collision between two collision objects";
            Sut.Description = "This message is broadcasted to every component of an instance that has a collision object,\nwhen the collision object collides with another collision object. For a script to take action\nwhen such a collision happens, it should check for this message in its on_message callback\nfunction.\nThis message only reports that a collision actually happened and will only be sent once per\ncolliding pair and frame.\nTo retrieve more detailed information, check for the contact_point_response instead.";
            Sut.Parameters = new [] {
                new ApiRefParameter {
                    Name = "other_id",
                    Description = "the id of the instance the collision object collided with",
                    Types = new [] { "hash" },
                },
                new ApiRefParameter {
                    Name = "other_position",
                    Description = "the world position of the instance the collision object collided with",
                    Types = new [] { "vector3" },
                },
                new ApiRefParameter {
                    Name = "other_group",
                    Description = "the collision group of the other collision object (hash)",
                    Types = new [] { "hash" },
                },
                new ApiRefParameter {
                    Name = "own_group",
                    Description = "the collision group of the own collision object (hash)",
                    Types = new [] { "hash" },
                },
            };
            Sut.ReturnValues = Array.Empty<ApiRefReturnValue>();

            var annotations = Sut.ToAnnotation();

            Assert.That(annotations, Is.EqualTo(
                "---This message is broadcasted to every component of an instance that has a collision object,\n" +
                "---when the collision object collides with another collision object. For a script to take action\n" +
                "---when such a collision happens, it should check for this message in its on_message callback\n" +
                "---function.\n" +
                "---This message only reports that a collision actually happened and will only be sent once per\n" +
                "---colliding pair and frame.\n" +
                "---To retrieve more detailed information, check for the contact_point_response instead.\n" +
                "---@class collision_response_msg\n" +
                "---@field other_id hash\n" +
                "---@field other_position vector3\n" +
                "---@field other_group hash\n" +
                "---@field own_group hash"
            ));
        }

        [Test]
        public void ToAnnotation_given_an_outgoing_message_returns_alias_annotation()
        {
            Sut.Type = "MESSAGE";
            Sut.OutgoingMessage = true;
            Sut.Name = "play_sound";
            Sut.Brief = "plays a sound";
            Sut.Description = "Post this message to a sound-component to make it play its sound. Multiple voices is supported. The limit is set to 32 voices per sound component.\nNote that gain is in linear scale, between 0 and 1.\nTo get the dB value from the gain, use the formula 20 * log(gain).\nInversely, to find the linear value from a dB value, use the formula\n10db/20.\nA sound will continue to play even if the game object the sound component belonged to is deleted. You can send a stop_sound to stop the sound.";
            Sut.Parameters = new [] {
                new ApiRefParameter {
                    Name = "delay",
                    Description = "delay in seconds before the sound starts playing, default is 0.",
                    Types = new [] { "number" },
                    Optional = true,
                },
                new ApiRefParameter {
                    Name = "gain",
                    Description = "sound gain between 0 and 1, default is 1.",
                    Types = new [] { "number" },
                    Optional = true,
                },
                new ApiRefParameter {
                    Name = "play_id",
                    Description = "the identifier of the sound, can be used to distinguish between consecutive plays from the same component.",
                    Types = new [] { "number" },
                    Optional = true,
                },
                new ApiRefParameter {
                    Name = "a_fake_required_param",
                    Description = "A fake required parameter added only for the sake of testing.",
                    Types = new [] { "string", "hash" },
                },
            };
            Sut.ReturnValues = Array.Empty<ApiRefReturnValue>();

            var annotations = Sut.ToAnnotation();

            Assert.That(annotations, Is.EqualTo(
                "---Post this message to a sound-component to make it play its sound. Multiple voices is supported. The limit is set to 32 voices per sound component.\n" +
                "---Note that gain is in linear scale, between 0 and 1.\n" +
                "---To get the dB value from the gain, use the formula 20 * log(gain).\n" +
                "---Inversely, to find the linear value from a dB value, use the formula\n" +
                "---10db/20.\n" +
                "---A sound will continue to play even if the game object the sound component belonged to is deleted. You can send a stop_sound to stop the sound.\n" +
                "---@class play_sound_msg\n" +
                "---@field delay number|nil\n" +
                "---@field gain number|nil\n" +
                "---@field play_id number|nil\n" +
                "---@field a_fake_required_param string|hash"
            ));
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

            Assert.That(annotations, Is.EqualTo(
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
                "function msg.post(receiver, message_id, message) end"
            ));
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

            Assert.That(annotations, Is.EqualTo(
                "---First line of the description.\n" +
                "---Second line of the description.\n" +
                "---@param required_param1 string|url|hash The first required parameter.\n" +
                "---@param optional_param1 string|hash|nil The first optional parameter.\n" +
                "---@param optional_param2 table|nil The second optional parameter.\n" +
                "---@overload fun(required_param1: string|url|hash, optional_param1: string|hash|nil)\n" +
                "---@overload fun(required_param1: string|url|hash)\n" +
                "function msg.something(required_param1, optional_param1, optional_param2) end"
            ));
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

            Assert.That(annotations, Is.EqualTo(
                "---First line of the description.\n" +
                "---Second line of the description.\n" +
                "---@param required_param1 string|url|hash The first required parameter.\n" +
                "---@param optional_param1 string|hash|nil The first optional parameter.\n" +
                "---@param optional_param2 table|nil The second optional parameter.\n" +
                "---@overload fun(required_param1: string|url|hash, optional_param1: string|hash|nil): url|string\n" +
                "---@overload fun(required_param1: string|url|hash): url|string\n" +
                "---@return url|string a new URL\n" +
                "function msg.something(required_param1, optional_param1, optional_param2) end"
            ));
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
