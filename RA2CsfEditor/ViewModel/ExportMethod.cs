using RA2CsfEditor.Command;
using RA2CsfEditor.Model;
using System;
using System.ComponentModel;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace RA2CsfEditor.ViewModel
{
    public partial class MainVM
    {
        public async Task ExportToCsf(string path)
        {
            _path = path;
            await ThisCSF.SaveToFile(path);
            Update();
        }
        public async Task ExportToJson(string path)
        {
            var jsObjs = new System.Collections.Generic.List<JsonObject>();
            MaxStatus = (from lbls in Labels select lbls.Labels.Length).Sum();
            Status = 0;
            var obj = new object();
            IsIndeterminate = false;
            for (int i = 0; i < Labels.Length; i++)
            {
                await Task.Run(() =>
                {
                    foreach (var (Label, jsonObj) in from Label in Labels[i].Labels
                                                     let jsonObj = new JsonObject
                        {
                            { "Label", Label.LabelString },
                            { "Value", new JsonArray( from Value values in Label.Values select (JsonValue)values.ValueString) }
                        }
                                                     select (Label, jsonObj))
                    {
                        if (!string.IsNullOrEmpty(Label.ExtraValue))
                            jsonObj.Add("ExtraValue", Label.ExtraValue);
                        lock (obj)
                        {
                            jsObjs.Add(jsonObj);
                            Status++;
                        }
                    }
                });
            }
            IsIndeterminate = true;
            using (var ts = new System.IO.StreamWriter(new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read)))
            {
                new JsonObject
                {
                    { "Version", ThisCSF.Header.Version },
                    { "Language", ThisCSF.Header.Language },
                    { "Strings", new JsonArray(jsObjs)}
                }.Save(ts);
            }
            IsIndeterminate = false;
        }
        public async Task ExportToXml(string path)
        {
            MaxStatus = (from lbls in Labels select lbls.Labels.Length).Sum();
            Status = 0;
            var obj = new object();
            // 创建XML文档
            var xml = new XmlDocument();
            // 写入基本XML信息
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));           
            // Root Node
            var root = xml.CreateElement("CSFfile"); 
            // CSF XML 文件版本
            root.SetAttribute("CSFXMLVer", "1.0");
            // CSF 文件版本
            root.SetAttribute("Version", ThisCSF.Header.Version.ToString());
            // CSF 语言信息
            root.SetAttribute("Language", ThisCSF.Header.Language.ToString());
            IsIndeterminate = false;
            for (int i = 0; i < Labels.Length; i++)
            {
                await Task.Run(() =>
                {
                    foreach (var (lbl, label) in from lbl in Labels[i].Labels
                                                 let label = xml.CreateElement("Label")
                                                 select (lbl, label))
                    {
                        foreach (var vstr in from vstr in lbl.Values select vstr)
                        {
                            var value = xml.CreateElement("Value");
                            value.SetAttribute("Content", vstr.ValueString);
                            label.AppendChild(value);
                        }
                        label.SetAttribute("Name", lbl.LabelString);
                        if (!string.IsNullOrEmpty(lbl.ExtraValue))
                            label.SetAttribute("ExtraValue", lbl.ExtraValue);
                        lock (obj)
                        {
                            root.AppendChild(label);
                            Status++;
                        }
                    }
                });
            }
            IsIndeterminate = true;
            using (var ts = new System.IO.StreamWriter(new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read)))
            {
                xml.AppendChild(root);
                xml.Save(ts);
            }
            IsIndeterminate = false;
        }
    }
}
