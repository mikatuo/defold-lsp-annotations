using App.Dtos;
using App.Parsers;
using FluentAssertions;

namespace App.Tests.Parsers
{
    [TestFixture]
    [Category("Unit")]
    public class DefoldParameterParserTest
    {
        [Test]
        public void Parse_can_extract_table_parameters_from_description_from_dl_dt_dd_tags()
        {
            // docs: https://defold.com/ref/stable/sound/#sound.play
            var parameter = new RawApiRefParameter {
                Name = "[play_properties]",
                Description = "<dl>\n<dt>optional table with properties:</dt>\n<dt><code>delay</code></dt>\n<dd><span class=\"type\">number</span> delay in seconds before the sound starts playing, default is 0.</dd>\n<dt><code>gain</code></dt>\n<dd><span class=\"type\">number</span> sound gain between 0 and 1, default is 1. The final gain of the sound will be a combination of this gain, the group gain and the master gain.</dd>\n<dt><code>pan</code></dt>\n<dd><span class=\"type\">number</span> sound pan between -1 and 1, default is 0. The final pan of the sound will be an addition of this pan and the sound pan.</dd>\n<dt><code>speed</code></dt>\n<dd><span class=\"type\">number</span> sound speed where 1.0 is normal speed, 0.5 is half speed and 2.0 is double speed. The final speed of the sound will be a multiplication of this speed and the sound speed.</dd>\n</dl>",
                Types = new[] { "table" },
            };

            var sut = new DefoldParameterParser(parameter);
            DefoldParameter result = sut.Parse();

            result.Should().BeEquivalentTo(new DefoldParameter {
                Name = "play_properties",
                Description = "optional table with properties",
                Types = new[] {
                    "{delay:number, gain:number, pan:number, speed:number}",
                },
                Optional = true,
            });
        }

        [Test]
        public void Parse_can_extract_table_parameters_from_description_from_ul_li_tags()
        {
            // docs: https://defold.com/ref/stable/http/?q=http.request#http.request
            var parameter = new RawApiRefParameter {
                Name = "[options]",
                Description = "optional table with request parameters. Supported entries:\n<ul>\n<li><span class=\"type\">number</span> <code>timeout</code>: timeout in seconds</li>\n<li><span class=\"type\">string</span> <code>path</code>: path on disc where to download the file. Only overwrites the path if status is 200</li>\n<li><span class=\"type\">boolean</span> <code>ignore_cache</code>: don't return cached data if we get a 304</li>\n<li><span class=\"type\">boolean</span> <code>chunked_transfer</code>: use chunked transfer encoding for https requests larger than 16kb. Defaults to true.</li>\n</ul>",
                Types = new[] { "table" },
            };

            var sut = new DefoldParameterParser(parameter);
            DefoldParameter result = sut.Parse();

            result.Should().BeEquivalentTo(new DefoldParameter {
                Name = "options",
                Description = "optional table with request parameters. Supported entries",
                Types = new[] {
                    "{timeout:number, path:string, ignore_cache:boolean, chunked_transfer:boolean}",
                },
                Optional = true,
            });
        }

        [Test]
        public void Parse_can_extract_function_parameter_types_from_description_from_dl_dt_dd_and_nested_ul_li_tags_in_between_the_dt_tags()
        {
            // docs: https://defold.com/ref/stable/sound/#sound.play
            var parameter = new RawApiRefParameter {
                Name = "[complete_function]",
                Description = "function to call when the sound has finished playing.\n<dl>\n<dt><code>self</code></dt>\n<dd><span class=\"type\">object</span> The current object.</dd>\n<dt><code>message_id</code></dt>\n<dd><span class=\"type\">hash</span> The name of the completion message, <code>\"sound_done\"</code>.</dd>\n<dt><code>message</code></dt>\n<dd><span class=\"type\">table</span> Information about the completion:</dd>\n</dl>\n<ul>\n<li><span class=\"type\">number</span> <code>play_id</code> - the sequential play identifier that was given by the sound.play function.</li>\n</ul>\n<dl>\n<dt><code>sender</code></dt>\n<dd><span class=\"type\">url</span> The invoker of the callback: the sound component.</dd>\n</dl>",
                Types = new[] { "function(self, message_id, message, sender))" },
            };

            var sut = new DefoldParameterParser(parameter);
            DefoldParameter result = sut.Parse();

            result.Should().BeEquivalentTo(new DefoldParameter {
                Name = "complete_function",
                Description = "function to call when the sound has finished playing.",
                Types = new[] {
                    "fun(self:object, message_id:hash, message:{play_id:number}, sender:url)",
                },
                Optional = true,
            });
        }

        [Test]
        public void Parse_can_extract_function_parameter_types_from_description_from_dl_dt_dd_and_nested_ul_li_tags_after_the_closing_dl_tag()
        {
            // docs: https://defold.com/ref/stable/http/?q=http.request#http.request
            var parameter = new RawApiRefParameter {
                Name = "callback",
                Description = "response callback function\n<dl>\n<dt><code>self</code></dt>\n<dd><span class=\"type\">object</span> The script instance</dd>\n<dt><code>id</code></dt>\n<dd><span class=\"type\">hash</span> Internal message identifier. Do not use!</dd>\n<dt><code>response</code></dt>\n<dd><span class=\"type\">table</span> The response data. Contains the fields:</dd>\n</dl>\n<ul>\n<li><span class=\"type\">number</span> <code>status</code>: the status of the response</li>\n<li><span class=\"type\">string</span> <code>response</code>: the response data (if not saved on disc)</li>\n<li><span class=\"type\">table</span> <code>headers</code>: all the returned headers</li>\n<li><span class=\"type\">string</span> <code>path</code>: the stored path (if saved to disc)</li>\n<li><span class=\"type\">string</span> <code>error</code>: if any unforeseen errors occurred (e.g. file I/O)</li>\n</ul>",
                Types = new[] { "function(self, id, response)" },
            };

            var sut = new DefoldParameterParser(parameter);
            DefoldParameter result = sut.Parse();

            result.Should().BeEquivalentTo(new DefoldParameter {
                Name = "callback",
                Description = "response callback function",
                Types = new[] {
                    "fun(self:object, id:hash, response:{status:number, response:string, headers:table, path:string, error:string})",
                },
            });
        }
    }
}
