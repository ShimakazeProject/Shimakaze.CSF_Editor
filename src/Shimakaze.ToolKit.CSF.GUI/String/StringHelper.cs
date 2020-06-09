using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Shimakaze.ToolKit.CSF.GUI.String
{
    public class StringHelper
    {
        public readonly static JsonSerializerOptions JsonSerializerOptions;
        static StringHelper()
        {
            JsonSerializerOptions = new JsonSerializerOptions();
            JsonSerializerOptions.AllowTrailingCommas = true;
            JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
            JsonSerializerOptions.WriteIndented = true;
        }
        public static async Task SerializeAsync(LocalString obj, Stream stream) => await JsonSerializer.SerializeAsync(stream, obj, JsonSerializerOptions);
        public static async Task<LocalString> Deserialize(Stream stream) => await JsonSerializer.DeserializeAsync<LocalString>(stream, JsonSerializerOptions);
    }
}
