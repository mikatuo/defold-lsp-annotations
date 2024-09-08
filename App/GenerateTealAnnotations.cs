using App.AnnotatorsTeal;
using App.Dtos;
using App.Parsers;

namespace App
{
    public class GenerateTealAnnotations
    {
        public IEnumerable<string> DefoldBaseAnnotations()
            => GetDefoldBaseAnnotations();

        public IEnumerable<string> ForApiReference(RawApiReference apiRef)
        {
            DefoldApi api = new DefoldDocsParser(apiRef).Parse();

            var extraDefinitions = ReplaceInlineRecordsWithType(api.Functions, api.Messages);

            yield return $"---{api.Brief}";

            if (api.Namespace == "builtins") {
                foreach (var function in api.Functions) {
                    var annotator = new FunctionTealAnnotator(function);
                    yield return $"\t---Docs: https://defold.com/ref/stable/{apiRef.Info.Namespace}/?q={function.Name}#{function.Name}";
                    yield return "\t---";
                    foreach (var line in annotator.GenerateAnnotations()) {
                        var trim = line.Trim();

                        if (trim.StartsWith("---")) {
                            yield return trim;
                        } else {
                            yield return $"global {trim}";
                        }
                    }
                }

                yield break;
            }

            var name = api.Namespace;

            if (IdentifierRenameMap.TryGetValue(name, out var newName)) {
                name = newName;
            }

            yield return $"global record {name}";

            bool first = true;

            // annotate functions
            foreach (var function in api.Functions) {
                // Skip functions without a . as they are examples in the documentation (e.g. on_input, on_message)
                if (!function.Name.Contains(".")) {
                    continue;
                }

                if (first) {
                    first = false;
                } else {
                    yield return "";
                }

                var annotator = new FunctionTealAnnotator(function);
                yield return $"\t---Docs: https://defold.com/ref/stable/{apiRef.Info.Namespace}/?q={function.Name}#{function.Name}";
                yield return "\t---";
                foreach (var line in annotator.GenerateAnnotations())
                    yield return line;
            }

            first = !first;

            // generate annotations for constants
            foreach (var element in apiRef.Elements) {
                switch (element.Type.ToLowerInvariant()) {
                    case "variable":
                        if (first) {
                            first = false;
                            yield return "";
                        }

                        foreach (var annotation in new VariableTealAnnotator(element).GenerateAnnotations()) {
                            yield return annotation;
                        }
                        break;
                }
            }

            yield return $"end";

            first = true;

            // annotate messages
            foreach (var message in api.Messages) {
                if (first) {
                    first = false;
                } else {
                    yield return "";
                }

                var annotator = new MessageTealAnnotator(message);
                foreach (var line in annotator.GenerateAnnotations())
                    yield return line;
            }

            first = true;

            foreach (var def in extraDefinitions) {
                if (first) {
                    first = false;
                    yield return "";
                }

                yield return def;
            }

            if (ExtraDefinitions.TryGetValue(api.Namespace, out var definitions)) {
                first = true;
                foreach (var def in definitions) {
                    if (first) {
                        first = false;
                        yield return "";
                    }

                    yield return def;
                }
            }
        }

        private IList<string> ReplaceInlineRecordsWithType(DefoldFunction[] functions, DefoldMessage[] messages)
        {
            var result = new List<string>();

            foreach (var func in functions) {
                foreach (var param in func.Parameters) {
                    GenerateTypesForInlineRecords(param.Types, () => $"{func.Name.Split(".").Last()}_{param.Name}", result);
                }

                foreach (var retVal in func.ReturnValues) {
                    GenerateTypesForInlineRecords(retVal.Types, () => $"{func.Name.Split(".").Last()}_return", result);
                }
            }

            foreach (var message in messages) {
                foreach (var param in message.Parameters) {
                    GenerateTypesForInlineRecords(param.Types, () => $"{message.Name.Split(".").Last()}_{param.Name}", result);
                }
            }

            return result;
        }

        private void GenerateTypesForInlineRecords(string[] typeList, Func<string> makeIdentifier, IList<string> result)
        {
            for (var i = 0; i < typeList.Length; i++) {
                var typeIn = typeList[i].Trim();

                var isRecord = typeIn.StartsWith("{") && typeIn.Contains(":") && typeIn.EndsWith("}");

                if (!isRecord) {
                    continue;
                }

                var name = makeIdentifier();
                typeList[i] = name;

                if (result.Count > 0) {
                    result.Add("");
                }

                GenerateRecordFromInlineRecord(result, typeIn, name);
            }
        }

        private static void GenerateRecordFromInlineRecord(IList<string> result, string typeIn, string name)
        {
            var types = typeIn.Trim('{', '}').Split(",");

            result.Add($"local record {name}");
            var usedIds = new HashSet<string>();

            foreach (var t in types) {
                var idWithType = t.Split(":");
                var id = idWithType[0].Trim();

                if (usedIds.Contains(id)) {
                    continue;
                }

                usedIds.Add(id);

                var type = idWithType[1]
                    .Split("|")
                    .Select(t => t.Trim())
                    .Select(t => IdentifierRenameMap.TryGetValue(t, out var typeReplacement) ? typeReplacement : t);

                result.Add($"\t{id}: {string.Join("|", type)}");
            }

            result.Add("end");
        }

        public static IEnumerable<string> GenerateFullDefinition(string outputDirectory)
        {
            var first = true;
            foreach (var api in workingApi) {
                if (first) {
                    first = false;
                } else {
                    yield return string.Empty;
                    yield return string.Empty;
                }

                var path = Path.Combine(outputDirectory, $"{api}.d.tl");
                var data = File.ReadAllText(path);

                if (api == "") {

                } else if (api == "sprite") {
                    var startOfDuplicateDefinition = data.IndexOf("local record play_flipbook_play_properties");
                    var endOfDuplicateDefinition = data.IndexOf("end", startOfDuplicateDefinition) + 3;

                    var before = data.Substring(0, startOfDuplicateDefinition).Trim();
                    var after = data.Substring(endOfDuplicateDefinition).Trim();

                    yield return before;

                    if (after.Length > 0) {
                        yield return string.Empty;
                        yield return after;
                    }
                    continue;
                }

                yield return data.Trim();
            }
        }

        #region Defold Base Types
        private IEnumerable<string> GetDefoldBaseAnnotations()
        {
            return new[] {
                "global type handle = number",
                "global type hashed = number",
                "global type constant = number",
                "global type bool = boolean",
                "global type float = number",
                "global type int = integer",
                "global record object end",
                "global record node end",
                "global record texture end",
                "global type array = {any}",
                "global type bufferstream = {number}",
                "",
                "global record vector3",
                "\tx: number",
                "\ty: number",
                "\tz: number",
                "\tmetamethod __add: function(vector3, vector3): vector3",
                "\tmetamethod __sub: function(vector3, vector3): vector3",
                "\tmetamethod __mul: function(vector3, number): vector3",
                "\tmetamethod __div: function(vector3, number): vector3",
                "end",
                "",
                "global record vector4",
                "\tx: number",
                "\ty: number",
                "\tz: number",
                "\tw: number",
                "\tmetamethod __add: function(vector4, vector4): vector4",
                "\tmetamethod __sub: function(vector4, vector4): vector4",
                "\tmetamethod __mul: function(vector4, number): vector4",
                "\tmetamethod __div: function(vector4, number): vector4",
                "end",
                "",
                "global record quaternion",
                "\tx: number",
                "\ty: number",
                "\tz: number",
                "\tw: number",
                "end",
                "",
                "global type vector = {number}",
                "",
                "global record matrix4",
                "\tc0: vector4",
                "\tc1: vector4",
                "\tc2: vector4",
                "\tc3: vector4",
                "\tm00: number",
                "\tm01: number",
                "\tm02: number",
                "\tm03: number",
                "\tm10: number",
                "\tm11: number",
                "\tm12: number",
                "\tm13: number",
                "\tm20: number",
                "\tm21: number",
                "\tm22: number",
                "\tm23: number",
                "\tm30: number",
                "\tm31: number",
                "\tm32: number",
                "\tm33: number",
                "\tmetamethod __mul: function(matrix4, matrix4): matrix4",
                "\tmetamethod __mul: function(matrix4, vector4): vector4",
                "end",
                "",
                "global record url",
                "\tsocket: number | string",
                "\tpath: string | hashed",
                "\tfragment: string | hashed",
                "end",
                "",
                "-- luasocket",
                "global record master end",
                "global record unconnected end",
                "global record client end",
                "",
                "-- render",
                "global type constant_buffer = {string:any}",
                "global record render_target end",
                "global record predicate end",
                "",
                "global record action_msg",
                "\tvalue: number",
                "\tpressed: boolean",
                "\treleased: boolean",
                "\trepeated: boolean",
                "\tx: number",
                "\ty: number",
                "\tscreen_x: number",
                "\tscreen_y: number",
                "\tdx: number",
                "\tdy: number",
                "\tscreen_dx: number",
                "\tscreen_dy: number",
                "\tgamepad: number",
                "\ttouch: {action_touch_input}",
                "\tacc_x: number",
                "\tacc_y: number",
                "\tacc_z: number",
                "end",
                "",
                "global record action_touch_input",
                "\tid: number",
                "\tpressed: boolean",
                "\treleased: boolean",
                "\ttap_count: number",
                "\tx: number",
                "\ty: number",
                "\tdx: number",
                "\tdy: number",
                "end",
            };
        }
        #endregion

        public static string[] workingApi = new string[] {
            "base_defold",
            "b2d_body",
            "b2d",
            "buffer",
            "builtins",
            "camera",
            "collection_factory",
            "collection_proxy",
            "collision_object",
            "crash",
            "factory",
            "game_object",
            "gui",
            "graphics",
            "html5",
            "http",
            "image",
            "json",
            "label",
            "liveupdate",
            "luasocket",
            "message",
            "model",
            "particle_effects",
            "profiler",
            "render",
            "resource",
            "sound",
            "sprite",
            "system",
            "tilemap",
            "timer",
            "vector_math",
            "window",
            "zlib"
        };

        public static IDictionary<string, string> IdentifierRenameMap = new Dictionary<string, string>
        {
            { "hash", "hashed" },
            { "repeat", "repeat_" },
            { "transform-bitmask", "transform_bitmask" },
            { "vmath.matrix4", "matrix4" },
            { "quat", "quaternion" },
            { "vmath.vector3", "vector3" },
            { "b2d.body", "b2Body" },
            { "create_declaration", "{create_declaration}" },
            { "function(self, event, data)", "function(self: any, event: hashed, data: table)" },
            { "function(self, node)", "function(self: any, node: any)" },
            { "function(self)", "function(self: any)" },
            { "function(self, request_id, result)", "function(self: any, request_id: any, result: table)" },
            // TODO process types within parameter function definitions
            { "function(self:object, id:hash, response:{status:number, response:string, headers:table, path:string, error:string})", "function(self:object, id:hashed, response: http_response)" },
            { "function(self:object, message_id:hash, message:{animation_id:hash, playback:constant}, sender:url)", "function(self:object, message_id:hashed, message:model_play_anim_complete_message, sender:url)" },
            { "function(self:object, message_id:hash, message:{current_tile:number, id:hash}, sender:url)", "function(self:object, message_id:hashed, message:sprite_playflipbook_complete_message, sender:url)" },
            { "function(self:object, message_id:hash, message:{play_id:number}, sender:url)", "function(self:object, message_id:hashed, message:sound_play_complete_message, sender:url)" },
            { "function(self:object, url:url, property:hash)", "function(self:object, url:url, property:hashed)" },
            { "function(self:object, node:hash, emitter:hash, state:constant)", "function(self:object, node:hashed, emitter:hashed, state:constant)" },
            { "function(self:object, id:hash, emitter:hash, state:constant)", "function(self:object, id:hashed, emitter:hashed, state:constant)" },
        };

        public static IDictionary<string, string[]> ExtraDefinitions = new Dictionary<string, string[]>
        {
            {"b2d.body", new[] {
                "global record b2World end",
                "",
                "global record b2BodyType",
                "\tSTATIC: constant",
                "\tKINEMATIC: constant",
                "\tDYNAMIC: constant",
                "end"
            }},
            {"http", new[] {
                "local record http_response",
                "\tresponse: string",
                "\theaders: table",
                "\tpath: string",
                "\terror: string",
                "end",
            }},
            {"model", new[] {
                "local record model_play_anim_complete_message",
                "\tanimation_id: hashed",
                "\tplayback: constant",
                "end",
            }},
            {"sprite", new[] {
                "local record sprite_playflipbook_complete_message",
                "\tcurrent_tile: number",
                "\tid: hashed",
                "end",
            }},
            {"sound", new[] {
                "local record sound_play_complete_message",
                "\tplay_id: number",
                "end",
            }},
        };
    }
}
