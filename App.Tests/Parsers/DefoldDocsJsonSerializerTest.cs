using App.Dtos;
using App.Parsers;
using App.Tests.Properties;
using FluentAssertions;
using System.Text;

namespace App.Tests.Parsers
{
    [TestFixture]
    [Category("Unit")]
    public class DefoldDocsJsonSerializerTest
    {
        [Test]
        public void Deserialize_given_msg_doc_json_returns_correct_api_reference()
        {
            var jsonString = ToString(Resources.msg_doc_1_4_1_json);

            RawApiReference result = DefoldDocsJsonSerializer.Deserialize(jsonString);

            result.Info.Should().BeEquivalentTo(new RawApiRefInfo {
                Namespace = "msg",
                Name = "Message",
                Brief = "Messaging API documentation",
                Description = "Functions for passing messages and constructing URL objects.",
                Path = "",
                File = "",
                Group = "SCRIPT",
            });
            result.Elements.Should().BeEquivalentTo(new[] {
                new RawApiRefElement{
                    Type = "FUNCTION",
                    Name = "msg.url",
                    Brief = "creates a new URL",
                    Description = "This is equivalent to <code>msg.url(nil)</code> or <code>msg.url(\"#\")</code>, which creates an url to the current\nscript component.",
                    ReturnValues = new [] {
                        new RawApiRefReturnValue {
                            Name = "url",
                            Description = "a new URL",
                            Types = new [] { "url" }
                        }
                    },
                    Parameters = Array.Empty<RawApiRefParameter>(),
                    Examples = "Create a new URL which will address the current script:\n<div class=\"codehilite\"><pre><span></span><code><span class=\"kd\">local</span> <span class=\"n\">my_url</span> <span class=\"o\">=</span> <span class=\"n\">msg</span><span class=\"p\">.</span><span class=\"n\">url</span><span class=\"p\">()</span>\n<span class=\"nb\">print</span><span class=\"p\">(</span><span class=\"n\">my_url</span><span class=\"p\">)</span> <span class=\"c1\">--&gt; url: [current_collection:/my_instance#my_component]</span>\n</code></pre></div>",
                    Replaces = "",
                    Error = "",
                    TParams = Array.Empty<string>(),
                    Members = Array.Empty<string>(),
                    Notes = Array.Empty<string>(),
                },
                new RawApiRefElement{
                    Type = "FUNCTION",
                    Name = "msg.url",
                    Brief = "creates a new URL from a string",
                    Description = "The format of the string must be <code>[socket:][path][#fragment]</code>, which is similar to a HTTP URL.\nWhen addressing instances:\n<ul>\n<li><code>socket</code> is the name of a valid world (a collection)</li>\n<li><code>path</code> is the id of the instance, which can either be relative the instance of the calling script or global</li>\n<li><code>fragment</code> would be the id of the desired component</li>\n</ul>\nIn addition, the following shorthands are available:\n<ul>\n<li><code>\".\"</code> the current game object</li>\n<li><code>\"#\"</code> the current component</li>\n</ul>",
                    ReturnValues = new [] {
                        new RawApiRefReturnValue {
                            Name = "url",
                            Description = "a new URL",
                            Types = new [] { "url" }
                        }
                    },
                    Parameters = new [] {
                        new RawApiRefParameter {
                            Name = "urlstring",
                            Description = "string to create the url from",
                            Types = new [] { "string" }
                        }
                    },
                    Examples = "<div class=\"codehilite\"><pre><span></span><code><span class=\"kd\">local</span> <span class=\"n\">my_url</span> <span class=\"o\">=</span> <span class=\"n\">msg</span><span class=\"p\">.</span><span class=\"n\">url</span><span class=\"p\">(</span><span class=\"s2\">&quot;#my_component&quot;</span><span class=\"p\">)</span>\n<span class=\"nb\">print</span><span class=\"p\">(</span><span class=\"n\">my_url</span><span class=\"p\">)</span> <span class=\"c1\">--&gt; url: [current_collection:/my_instance#my_component]</span>\n\n<span class=\"kd\">local</span> <span class=\"n\">my_url</span> <span class=\"o\">=</span> <span class=\"n\">msg</span><span class=\"p\">.</span><span class=\"n\">url</span><span class=\"p\">(</span><span class=\"s2\">&quot;my_collection:/my_sub_collection/my_instance#my_component&quot;</span><span class=\"p\">)</span>\n<span class=\"nb\">print</span><span class=\"p\">(</span><span class=\"n\">my_url</span><span class=\"p\">)</span> <span class=\"c1\">--&gt; url: [my_collection:/my_sub_collection/my_instance#my_component]</span>\n\n<span class=\"kd\">local</span> <span class=\"n\">my_url</span> <span class=\"o\">=</span> <span class=\"n\">msg</span><span class=\"p\">.</span><span class=\"n\">url</span><span class=\"p\">(</span><span class=\"s2\">&quot;my_socket:&quot;</span><span class=\"p\">)</span>\n<span class=\"nb\">print</span><span class=\"p\">(</span><span class=\"n\">my_url</span><span class=\"p\">)</span> <span class=\"c1\">--&gt; url: [my_collection:]</span>\n</code></pre></div>",
                    Replaces = "",
                    Error = "",
                    TParams = Array.Empty<string>(),
                    Members = Array.Empty<string>(),
                    Notes = Array.Empty<string>(),
                },
                new RawApiRefElement{
                    Type = "FUNCTION",
                    Name = "msg.url",
                    Brief = "creates a new URL from separate arguments",
                    Description = "creates a new URL from separate arguments",
                    ReturnValues = new [] {
                        new RawApiRefReturnValue {
                            Name = "url",
                            Description = "a new URL",
                            Types = new [] { "url" }
                        }
                    },
                    Parameters = new [] {
                        new RawApiRefParameter {
                            Name = "[socket]",
                            Description = "socket of the URL",
                            Types = new [] { "string", "hash" }
                        },
                        new RawApiRefParameter {
                            Name = "[path]",
                            Description = "path of the URL",
                            Types = new [] { "string", "hash" }
                        },
                        new RawApiRefParameter {
                            Name = "[fragment]",
                            Description = "fragment of the URL",
                            Types = new [] { "string", "hash" }
                        }
                    },
                    Examples = "<div class=\"codehilite\"><pre><span></span><code><span class=\"kd\">local</span> <span class=\"n\">my_socket</span> <span class=\"o\">=</span> <span class=\"s2\">&quot;main&quot;</span> <span class=\"c1\">-- specify by valid name</span>\n<span class=\"kd\">local</span> <span class=\"n\">my_path</span> <span class=\"o\">=</span> <span class=\"n\">hash</span><span class=\"p\">(</span><span class=\"s2\">&quot;/my_collection/my_gameobject&quot;</span><span class=\"p\">)</span> <span class=\"c1\">-- specify as string or hash</span>\n<span class=\"kd\">local</span> <span class=\"n\">my_fragment</span> <span class=\"o\">=</span> <span class=\"s2\">&quot;component&quot;</span> <span class=\"c1\">-- specify as string or hash</span>\n<span class=\"kd\">local</span> <span class=\"n\">my_url</span> <span class=\"o\">=</span> <span class=\"n\">msg</span><span class=\"p\">.</span><span class=\"n\">url</span><span class=\"p\">(</span><span class=\"n\">my_socket</span><span class=\"p\">,</span> <span class=\"n\">my_path</span><span class=\"p\">,</span> <span class=\"n\">my_fragment</span><span class=\"p\">)</span>\n\n<span class=\"nb\">print</span><span class=\"p\">(</span><span class=\"n\">my_url</span><span class=\"p\">)</span> <span class=\"c1\">--&gt; url: [main:/my_collection/my_gameobject#component]</span>\n<span class=\"nb\">print</span><span class=\"p\">(</span><span class=\"n\">my_url</span><span class=\"p\">.</span><span class=\"n\">socket</span><span class=\"p\">)</span> <span class=\"c1\">--&gt; 786443 (internal numeric value)</span>\n<span class=\"nb\">print</span><span class=\"p\">(</span><span class=\"n\">my_url</span><span class=\"p\">.</span><span class=\"n\">path</span><span class=\"p\">)</span> <span class=\"c1\">--&gt; hash: [/my_collection/my_gameobject]</span>\n<span class=\"nb\">print</span><span class=\"p\">(</span><span class=\"n\">my_url</span><span class=\"p\">.</span><span class=\"n\">fragment</span><span class=\"p\">)</span> <span class=\"c1\">--&gt; hash: [component]</span>\n</code></pre></div>",
                    Replaces = "",
                    Error = "",
                    TParams = Array.Empty<string>(),
                    Members = Array.Empty<string>(),
                    Notes = Array.Empty<string>(),
                },
                new RawApiRefElement{
                    Type = "FUNCTION",
                    Name = "msg.post",
                    Brief = "posts a message to a receiving URL",
                    Description = "Post a message to a receiving URL. The most common case is to send messages\nto a component. If the component part of the receiver is omitted, the message\nis broadcast to all components in the game object.\nThe following receiver shorthands are available:\n<ul>\n<li><code>\".\"</code> the current game object</li>\n<li><code>\"#\"</code> the current component</li>\n</ul>\n<span class=\"icon-attention\"></span> There is a 2 kilobyte limit to the message parameter table size.",
                    ReturnValues = Array.Empty<RawApiRefReturnValue>(),
                    Parameters = new [] {
                        new RawApiRefParameter {
                            Name = "receiver",
                            Description = "The receiver must be a string in URL-format, a URL object or a hashed string.",
                            Types = new [] { "string", "url", "hash" }
                        },
                        new RawApiRefParameter {
                            Name = "message_id",
                            Description = "The id must be a string or a hashed string.",
                            Types = new [] { "string", "hash" }
                        },
                        new RawApiRefParameter {
                            Name = "[message]",
                            Description = "a lua table with message parameters to send.",
                            Types = new [] { "table", "nil" }
                        }
                    },
                    Examples = "Send \"enable\" to the sprite \"my_sprite\" in \"my_gameobject\":\n<div class=\"codehilite\"><pre><span></span><code><span class=\"n\">msg</span><span class=\"p\">.</span><span class=\"n\">post</span><span class=\"p\">(</span><span class=\"s2\">&quot;my_gameobject#my_sprite&quot;</span><span class=\"p\">,</span> <span class=\"s2\">&quot;enable&quot;</span><span class=\"p\">)</span>\n</code></pre></div>\n\nSend a \"my_message\" to an url with some additional data:\n<div class=\"codehilite\"><pre><span></span><code><span class=\"kd\">local</span> <span class=\"n\">params</span> <span class=\"o\">=</span> <span class=\"p\">{</span><span class=\"n\">my_parameter</span> <span class=\"o\">=</span> <span class=\"s2\">&quot;my_value&quot;</span><span class=\"p\">}</span>\n<span class=\"n\">msg</span><span class=\"p\">.</span><span class=\"n\">post</span><span class=\"p\">(</span><span class=\"n\">my_url</span><span class=\"p\">,</span> <span class=\"s2\">&quot;my_message&quot;</span><span class=\"p\">,</span> <span class=\"n\">params</span><span class=\"p\">)</span>\n</code></pre></div>",
                    Replaces = "",
                    Error = "",
                    TParams = Array.Empty<string>(),
                    Members = Array.Empty<string>(),
                    Notes = Array.Empty<string>(),
                }
            });
        }

        string ToString(byte[] jsonBytes)
            => Encoding.UTF8.GetString(jsonBytes);
    }
}
