using App.Annotators;
using App.Dtos;
using App.Parsers;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Xml.Linq;

namespace App
{
    public class GenerateLuaAnnotations
    {
        public IEnumerable<string> DefoldBaseAnnotations()
            => GetDefoldBaseAnnotations();

        public IEnumerable<string> ForApiReference(RawApiReference apiRef)
        {
            DefoldApi api = new DefoldDocsParser(apiRef).Parse();

            yield return $"---@meta"; // https://github.com/sumneko/lua-language-server/wiki/Annotations#meta

            // define the module
            yield return "";
            yield return $"---{api.Brief}";
            yield return $"---@class {api.Namespace}";
            yield return $"{api.Namespace} = {{}}";
            yield return "";

            // annotate functions
            foreach (var function in api.Functions) {
                var annotator = new FunctionLuaAnnotator(function);
                yield return $"---Docs: https://defold.com/ref/stable/{apiRef.Info.Namespace}/?q={function.Name}#{function.Name}";
                yield return "---";
                foreach (var line in annotator.GenerateAnnotations())
                    yield return line;
                yield return "";
            }

            // annotate messages
            foreach (var message in api.Messages) {
                var annotator = new MessageLuaAnnotator(message);
                foreach (var line in annotator.GenerateAnnotations())
                    yield return line;
                yield return "";
            }

            // generate annotations for messages, constants
            foreach (var element in apiRef.Elements) {
                var annotations = element.ToAnnotation();
                if (annotations is null)
                    continue;
                yield return annotations;
                yield return "";
            }
        }

        #region Defold Base Types
        string[] GetDefoldBaseAnnotations()
        {
            return new [] {
                "---@class vector3",
                "---@operator add(vector3): vector3",
                "---@operator sub(vector3): vector3",
                "---@operator mul(number): vector3",
                "---@operator div(number): vector3",
                "---@field x number",
                "---@field y number",
                "---@field z number",
                "",
                "---@class vector4",
                "---@operator add(vector4): vector4",
                "---@operator sub(vector4): vector4",
                "---@operator mul(number): vector4",
                "---@operator div(number): vector4",
                "---@field x number",
                "---@field y number",
                "---@field z number",
                "---@field w number",
                "",
                "---@class quaternion",
                "---@field x number",
                "---@field y number",
                "---@field z number",
                "---@field w number",
                "",
                "---@alias quat quaternion",
                "",
                "---@class url",
                "---@field socket number|string",
                "---@field path string|hash",
                "---@field fragment string|hash",
                "",
                "---@class hash : userdata",
                "---@class constant : userdata",
                "---@alias bool boolean",
                "---@alias float number",
                "---@alias object userdata",
                "---@alias matrix4 userdata",
                "---@class node : userdata",
                "",
                "---@alias vector vector4",
                "",
                "-- luasocket",
                "---@alias master userdata",
                "---@alias unconnected userdata",
                "---@alias client userdata",
                "",
                "-- render",
                "---@alias constant_buffer userdata",
                "---@alias render_target userdata",
                "---@alias predicate userdata",
            };
        }
        #endregion
    }
}
