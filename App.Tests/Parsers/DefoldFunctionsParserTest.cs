using App.Dtos;
using App.Parsers;
using FluentAssertions;

namespace App.Tests.Parsers
{
    [TestFixture]
    [Category("Unit")]
    public class DefoldFunctionsParserTest
    {
        [Test]
        public void Parse_given_multiple_functions_parses_them_with_overloads()
        {
            RawApiRefElement[] elements = VmathLerpElements();

            var sut = new DefoldFunctionsParser(elements);
            DefoldFunction[] result = sut.Parse();

            result.Should().BeEquivalentTo(new[] {
                new DefoldFunction {
                    Name = "vmath.lerp",
                    Brief = "lerps between two vectors",
                    Description = new [] {
                        "Linearly interpolate between two vectors. The function",
                        "treats the vectors as positions and interpolates between",
                        "the positions in a straight line. Lerp is useful to describe",
                        "transitions from one place to another over time.",
                        "The function does not clamp t between 0 and 1.",
                    },
                    Parameters = new [] {
                        new DefoldParameter {
                            Name = "t",
                            Description = "interpolation parameter, 0-1",
                            Types = new [] { "number" },
                        },
                        new DefoldParameter {
                            Name = "v1",
                            Description = "vector to lerp from",
                            Types = new [] { "vector3", "vector4" },
                        },
                        new DefoldParameter {
                            Name = "v2",
                            Description = "vector to lerp to",
                            Types = new [] { "vector3", "vector4" },
                        },
                    },
                    ReturnValues = new [] {
                        new DefoldReturnValue {
                            Name = "v",
                            Description = "the lerped vector",
                            Types = new [] { "vector3", "vector4" },
                        },
                    },
                    Overloads = new [] {
                        new DefoldFunctionOverload {
                            Parameters = new [] {
                                new DefoldParameter {
                                    Description = "interpolation parameter, 0-1",
                                    Name = "t",
                                    Types = new [] { "number" },
                                },
                                new DefoldParameter {
                                    Description = "quaternion to lerp from",
                                    Name = "q1",
                                    Types = new [] { "quaternion" },
                                },
                                new DefoldParameter {
                                    Description = "quaternion to lerp to",
                                    Name = "q2",
                                    Types = new [] { "quaternion" },
                                }
                            },
                            ReturnValues = new [] {
                                new DefoldReturnValue {
                                    Description = "the lerped quaternion",
                                    Name = "q",
                                    Types = new [] { "quaternion" },
                                }
                            }
                        },
                        new DefoldFunctionOverload {
                            Parameters = new [] {
                                new DefoldParameter {
                                    Description = "interpolation parameter, 0-1",
                                    Name = "t",
                                    Types = new [] { "number" },
                                },
                                new DefoldParameter {
                                    Description = "number to lerp from",
                                    Name = "n1",
                                    Types = new [] { "number" },
                                },
                                new DefoldParameter {
                                    Description = "number to lerp to",
                                    Name = "n2",
                                    Types = new [] { "number" },
                                }
                            },
                            ReturnValues = new [] {
                                new DefoldReturnValue {
                                    Description = "the lerped number",
                                    Name = "n",
                                    Types = new [] { "number" },
                                }
                            }
                        }
                    },
                    Examples = "TODO",
                }
            });
        }

        static RawApiRefElement[] VmathLerpElements()
        {
            return new[] {
                new RawApiRefElement {
                    Type = "FUNCTION",
                    Name = "vmath.lerp",
                    Brief = "lerps between two vectors",
                    Description = "Linearly interpolate between two vectors. The function\ntreats the vectors as positions and interpolates between\nthe positions in a straight line. Lerp is useful to describe\ntransitions from one place to another over time.\n<span class=\"icon-attention\"></span> The function does not clamp t between 0 and 1.",
                    ReturnValues = new [] {
                        new RawApiRefReturnValue {
                            Name = "v",
                            Description = "the lerped vector",
                            Types = new [] {
                                "vector3",
                                "vector4"
                            }
                        }
                    },
                    Parameters = new [] {
                        new RawApiRefParameter {
                            Name = "t",
                            Description = "interpolation parameter, 0-1",
                            Types = new [] {
                                "number"
                            }
                        },
                        new RawApiRefParameter {
                            Name = "v1",
                            Description = "vector to lerp from",
                            Types = new [] {
                                "vector3",
                                "vector4"
                            }
                        },
                        new RawApiRefParameter {
                            Name = "v2",
                            Description = "vector to lerp to",
                            Types = new [] {
                                "vector3",
                                "vector4"
                            }
                        }
                    },
                    Examples = "",
                },
                new RawApiRefElement {
                    Type = "FUNCTION",
                    Name = "vmath.lerp",
                    Brief = "lerps between two quaternions",
                    Description = "Linearly interpolate between two quaternions. Linear\ninterpolation of rotations are only useful for small\nrotations. For interpolations of arbitrary rotations,\n<a href=\"/ref/vmath#vmath.slerp\">vmath.slerp</a> yields much better results.\n<span class=\"icon-attention\"></span> The function does not clamp t between 0 and 1.",
                    ReturnValues = new [] {
                        new RawApiRefReturnValue {
                            Name = "q",
                            Description = "the lerped quaternion",
                            Types = new [] {
                                "quaternion"
                            }
                        }
                    },
                    Parameters = new [] {
                        new RawApiRefParameter {
                            Name = "t",
                            Description = "interpolation parameter, 0-1",
                            Types = new [] {
                                "number"
                            }
                        },
                        new RawApiRefParameter {
                            Name = "q1",
                            Description = "quaternion to lerp from",
                            Types = new [] {
                                "quaternion"
                            }
                        },
                        new RawApiRefParameter {
                            Name = "q2",
                            Description = "quaternion to lerp to",
                            Types = new [] {
                                "quaternion"
                            }
                        }
                    },
                    Examples = "",
                },
                new RawApiRefElement {
                    Type = "FUNCTION",
                    Name = "vmath.lerp",
                    Brief = "lerps between two numbers",
                    Description = "Linearly interpolate between two values. Lerp is useful\nto describe transitions from one value to another over time.\n<span class=\"icon-attention\"></span> The function does not clamp t between 0 and 1.",
                    ReturnValues = new [] {
                        new RawApiRefReturnValue {
                            Name = "n",
                            Description = "the lerped number",
                            Types = new [] {
                                "number"
                            }
                        }
                        },
                        Parameters = new [] {
                        new RawApiRefParameter {
                            Name = "t",
                            Description = "interpolation parameter, 0-1",
                            Types = new [] {
                                "number"
                            }
                        },
                        new RawApiRefParameter {
                            Name = "n1",
                            Description = "number to lerp from",
                            Types = new [] {
                                "number"
                            }
                        },
                        new RawApiRefParameter {
                            Name = "n2",
                            Description = "number to lerp to",
                            Types = new [] {
                                "number"
                            }
                        }
                    },
                    Examples = "",
                }
            };
        }
    }
}
