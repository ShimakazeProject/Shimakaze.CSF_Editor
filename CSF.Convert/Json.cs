using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Convert
{
    public static class Json
    {
        public static bool Export(Model.File file,string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var sw = new StreamWriter(fs))
            {
                sw.WriteLine("{");
                {
                    int layout = 1;// 表示层数
                    #region Header
                    sw.WriteLine(new string(' ', layout * 4) + "\"head\": {");
                    layout++;
                    {
                        sw.WriteLine("{0}\"version\": {1},", new string(' ', layout * 4), file.Version);
                        sw.WriteLine("{0}\"language\": \"{1}\"", new string(' ', layout * 4), file.Language.ToString());
                    }
                    layout--;
                    sw.WriteLine("{0}{1},", new string(' ', layout * 4), "}");
                    #endregion
                    #region Body
                    sw.WriteLine(new string(' ', layout * 4) + "\"data\": [");
                    layout++;
                    for (int j = 0; j < file.Labels.Length; j++)// 遍历标签
                    {
                        Model.Label label = file.Labels[j];
                        sw.WriteLine(new string(' ', layout * 4) + "{");
                        layout++;
                        {
                            sw.WriteLine("{0}\"name\": \"{1}\",", new string(' ', layout * 4), label.LabelName);
                            if (label.LabelValues.Length == 1)// 如果值只有一个
                            {
                                var value = label.LabelValues[0];
                                sw.WriteLine("{0}\"{1}\": \"{2}\",", new string(' ', layout * 4), "value", value.ValueString.Replace("\n", @"\n"));
                                if (value.IsWString)// 是宽字符串
                                {
                                    sw.WriteLine("{0}\"{1}\": \"{2}\",", new string(' ', layout * 4), "extra", value.ExtraString);
                                }
                                sw.WriteLine("{0}\"{1}\": {2}", new string(' ', layout * 4), "wstring", value.IsWString.ToString().ToLowerInvariant());
                            }
                            else if (label.LabelValues.Length > 1)// 如果值多于一个
                            {
                                sw.WriteLine(new string(' ', layout * 4) + "\"values\": [");
                                layout++;
                                for (int i = 0; i < label.LabelValues.Length; i++)
                                {
                                    Model.Value value = label.LabelValues[i];
                                    sw.WriteLine(new string(' ', layout * 4) + "{");
                                    layout++;
                                    {
                                        sw.WriteLine("{0}\"{1}\": \"{2}\",", new string(' ', layout * 4), "value", value.ValueString.Replace("\n", @"\n"));
                                        if (value.IsWString)// 是宽字符串
                                        {
                                            sw.WriteLine("{0}\"{1}\": \"{2}\",", new string(' ', layout * 4), "extra", value.ExtraString);
                                        }
                                        sw.WriteLine("{0}\"{1}\": {2}", new string(' ', layout * 4), "wstring", value.IsWString.ToString().ToLowerInvariant());
                                    }
                                    layout--;
                                    if (i != label.LabelValues.Length - 1) sw.WriteLine(new string(' ', layout * 4) + "},");
                                    else sw.WriteLine(new string(' ', layout * 4) + "}");
                                }
                                layout--;
                                sw.WriteLine(new string(' ', layout * 4) + "]");
                            }
                            else return false;// 你在逗我..?
                        }
                        layout--;
                        if (j != file.Labels.Length - 1) sw.WriteLine(new string(' ', layout * 4) + "},");
                        else sw.WriteLine(new string(' ', layout * 4) + "}");
                    }
                    layout--;
                    sw.WriteLine(new string(' ', layout * 4) + "]");
                    #endregion
                }
                sw.WriteLine("}");
            }
            return true;
        }
        public static Model.File Import(string path)
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
        public static string ExportName => "导出到Json文件";
        public static string ImportName => "从Json文件导入";
    }
}
