using App.Dtos;
using App.Parsers;
using FluentAssertions;

namespace App.Tests.Parsers
{
    [TestFixture]
    [Category("Unit")]
    public class DefoldReturnValueParserTest
    {
        [Test]
        public void Parse_can_extract_table_parameters_from_description_from_dl_dt_dd_tags()
        {
            // docs: https://defold.com/ref/stable/sys/#sys.get_sys_info
            var returnValue = new RawApiRefReturnValue {
                Name = "sys_info",
                Description = "table with system information in the following fields:\n<dl>\n<dt><code>device_model</code></dt>\n<dd><span class=\"type\">string</span> <span class=\"icon-ios\"></span><span class=\"icon-android\"></span> Only available on iOS and Android.</dd>\n<dt><code>manufacturer</code></dt>\n<dd><span class=\"type\">string</span> <span class=\"icon-ios\"></span><span class=\"icon-android\"></span> Only available on iOS and Android.</dd>\n<dt><code>system_name</code></dt>\n<dd><span class=\"type\">string</span> The system name: \"Darwin\", \"Linux\", \"Windows\", \"HTML5\", \"Android\" or \"iPhone OS\"</dd>\n<dt><code>system_version</code></dt>\n<dd><span class=\"type\">string</span> The system OS version.</dd>\n<dt><code>api_version</code></dt>\n<dd><span class=\"type\">string</span> The API version on the system.</dd>\n<dt><code>language</code></dt>\n<dd><span class=\"type\">string</span> Two character ISO-639 format, i.e. \"en\".</dd>\n<dt><code>device_language</code></dt>\n<dd><span class=\"type\">string</span> Two character ISO-639 format (i.e. \"sr\") and, if applicable, followed by a dash (-) and an ISO 15924 script code (i.e. \"sr-Cyrl\" or \"sr-Latn\"). Reflects the device preferred language.</dd>\n<dt><code>territory</code></dt>\n<dd><span class=\"type\">string</span> Two character ISO-3166 format, i.e. \"US\".</dd>\n<dt><code>gmt_offset</code></dt>\n<dd><span class=\"type\">number</span> The current offset from GMT (Greenwich Mean Time), in minutes.</dd>\n<dt><code>device_ident</code></dt>\n<dd><span class=\"type\">string</span> This value secured by OS. <span class=\"icon-ios\"></span> \"identifierForVendor\" on iOS. <span class=\"icon-android\"></span> \"android_id\" on Android. On Android, you need to add <code>READ_PHONE_STATE</code> permission to be able to get this data. We don't use this permission in Defold.</dd>\n<dt><code>user_agent</code></dt>\n<dd><span class=\"type\">string</span> <span class=\"icon-html5\"></span> The HTTP user agent, i.e. \"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_3) AppleWebKit/602.4.8 (KHTML, like Gecko) Version/10.0.3 Safari/602.4.8\"</dd>\n</dl>",
                Types = new[] { "table" },
            };

            var sut = new DefoldReturnValueParser(returnValue);
            DefoldReturnValue result = sut.Parse();

            result.Should().BeEquivalentTo(new DefoldReturnValue {
                Name = "sys_info",
                Description = "table with system information in the following fields",
                Types = new[] {
                    "{device_model:string, manufacturer:string, system_name:string, system_version:string, api_version:string, language:string, device_language:string, territory:string, gmt_offset:number, device_ident:string, user_agent:string}",
                },
            });
        }
    }
}
