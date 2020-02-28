using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Text;

namespace CSF.Plugin.Convert
{
    public class JsonImport : FilePlugin
    {
        public override string Name => "从Json文件导入";

        public override string Description => "从储存特定的Json字符串的文件中还原出CSF文件";

        public override PluginType PluginType => PluginType.IMPORTER;

        public override string ExecuteName => nameof(Execute);

        public override string Filter => "Json配置文件(*.json)|*.json";

        public static Model.File Execute(string path)
        {
            JsonValue json;
            {
                var jsonStrBuilder = new StringBuilder();
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var sr = new StreamReader(fs);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    jsonStrBuilder.Append(line.Split("//")[0]);
                }
                json = JsonValue.Parse(jsonStrBuilder.ToString());
            }
            JsonValue headJson = json["head"];
            int label_count = 0;
            int string_count = 0;
            var list = new List<Model.Label>();
            var file = new Model.File();
            foreach (JsonObject labelJson in json["data"] as JsonArray)
            {
                label_count++;
                string labelName = labelJson["name"];
                var valueList = new List<Model.Value>();

                if (labelJson.TryGetValue("values", out JsonValue values)) // 有多个值
                    foreach (JsonObject valueJson in values as JsonArray)
                    {
                        string_count++;
                        bool wstring = valueJson["wstring"];
                        string value = valueJson["value"];
                        if (wstring) valueList.Add(new Model.Value(value, valueJson["extra"]));
                        else valueList.Add(new Model.Value(value));
                    }
                else // 没有多个值
                {
                    string_count++;
                    bool wstring = labelJson["wstring"];
                    string value = labelJson["value"];
                    if (wstring) valueList.Add(new Model.Value(value, labelJson["extra"]));
                    else valueList.Add(new Model.Value(value));
                }
                list.Add(new Model.Label(labelName, valueList.ToArray()));
            }
            file.Labels = list.ToArray();
            file.Flag = " FSC";
            file.LabelCount = label_count;
            file.StringCount = string_count;
            file.Language = (Model.CsfLanguage)Enum.Parse(typeof(Model.CsfLanguage), headJson["language"]);
            file.Version = headJson["version"];
            file.Unknow = new byte[4];
            return file;
        }
    }
}
