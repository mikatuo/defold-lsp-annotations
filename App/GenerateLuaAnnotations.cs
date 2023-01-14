using Core;

namespace App
{
    public class GenerateLuaAnnotations
    {
        public IEnumerable<string> DefoldBaseAnnotations()
            => GetDefoldBaseAnnotations();

        public IEnumerable<string> ForApiReference(DefoldApiReference apiRef)
        {
            // define the module
            yield return $"---{apiRef.Info.Brief}";
            foreach (var line in apiRef.Info.DescriptionAnnotation())
                yield return line;
            //yield return $"---@class {apiRef.Info.Namespace}";
            yield return $"{apiRef.Info.Namespace} = {{}}";

            // generate annotations for functions, messages, constants
            foreach (var element in apiRef.Elements) {
                if (element.Type.ToLowerInvariant() == "function") {
                    yield return $"---Docs: https://defold.com/ref/stable/{apiRef.Info.Namespace}/?q={element.Name}#{element.Name}";
                    yield return "---";
                }
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
                "---@field x number",
                "---@field y number",
                "---@field z number",
                "",
                "---@class vector4",
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
                "---@field socket",
                "---@field path",
                "---@field fragment",
                "",
                "---@alias hash userdata",
                "---@alias constant userdata",
                "---@alias bool boolean",
                "---@alias float number",
                "---@alias object userdata",
                "---@alias matrix4 userdata",
                "---@alias node userdata",
                "",
                "--mb use number instead of vector4",
                "---@alias vector vector4",
                "",
                "--luasocket",
                "---@alias master userdata",
                "---@alias unconnected userdata",
                "---@alias client userdata",
                "",
                "--render",
                "---@alias constant_buffer userdata",
                "---@alias render_target userdata",
                "---@alias predicate userdata",
            };
        }
        #endregion
    }
}
