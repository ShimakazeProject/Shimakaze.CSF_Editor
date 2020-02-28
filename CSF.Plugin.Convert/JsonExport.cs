using System;
using System.IO;

namespace CSF.Plugin.Convert
{
    public class JsonExport :  FilePlugin
    {
        public override string Name => "导出到Json文件";

        public override string Description =>"将CSF文件转换为特定的Json字符串并保存到文件";

        public override PluginType PluginType => PluginType.EXPORTER;

        public override string ExecuteName => nameof(Execute);

        public override string Filter => "Json配置文件(*.json)|*.json";
        public static bool Execute(Model.File file, string path)
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
    }
}
