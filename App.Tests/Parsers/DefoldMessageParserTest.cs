using App.Dtos;
using App.Parsers;
using FluentAssertions;

namespace App.Tests.Parsers
{
    [TestFixture]
    [Category("Unit")]
    public class DefoldMessageParserTest
    {
        [Test]
        public void Parse_given_a_message_parses_it_as_incoming_message()
        {
            var message = new RawApiRefElement{
                Type = "MESSAGE",
                Name = "animation_done",
                Brief = "reports that an animation has completed",
                Description = "This message is sent to the sender of a <code>play_animation</code> message when the\nanimation has completed.\nNote that this message is sent only for animations that play with the following\nplayback modes:\n<ul>\n<li>Once Forward</li>\n<li>Once Backward</li>\n<li>Once Ping Pong</li>\n</ul>\nSee <a href=\"#play_animation\">play_animation</a> for more information and examples of how to use\nthis message.",
                Parameters = new [] {
                    new RawApiRefParameter {
                        Name = "current_tile",
                        Description = "the current tile of the sprite",
                        Types = new [] { "number" },
                    },
                    new RawApiRefParameter {
                        Name = "id",
                        Description = "id of the animation that was completed",
                        Types = new [] { "hash" },
                    },
                },
                Examples = "Create a new URL which will address the current script:\n<div class=\"codehilite\"><pre><span></span><code><span class=\"kd\">local</span> <span class=\"n\">my_url</span> <span class=\"o\">=</span> <span class=\"n\">msg</span><span class=\"p\">.</span><span class=\"n\">url</span><span class=\"p\">()</span>\n<span class=\"nb\">print</span><span class=\"p\">(</span><span class=\"n\">my_url</span><span class=\"p\">)</span> <span class=\"c1\">--&gt; url: [current_collection:/my_instance#my_component]</span>\n</code></pre></div>",
            };

            var sut = new DefoldMessageParser(message);
            DefoldMessage result = sut.Parse();

            result.Should().BeEquivalentTo(new DefoldMessage {
                Name = "animation_done",
                Brief = "reports that an animation has completed",
                Description = new [] {
                    "This message is sent to the sender of a <code>play_animation</code> message when the",
                    "animation has completed.",
                    "Note that this message is sent only for animations that play with the following",
                    "playback modes:",
                    "",
                    "Once Forward",
                    "",
                    "Once Backward",
                    "",
                    "Once Ping Pong",
                    "",
                    "See play_animation for more information and examples of how to use",
                    "this message.",
                },
                Parameters = new [] {
                    new DefoldParameter {
                        Name = "current_tile",
                        Description = "the current tile of the sprite",
                        Types = new [] { "number" },
                    },
                    new DefoldParameter {
                        Name = "id",
                        Description = "id of the animation that was completed",
                        Types = new [] { "hash" },
                    },
                },
                Incoming = true,
            });
        }
    }
}
