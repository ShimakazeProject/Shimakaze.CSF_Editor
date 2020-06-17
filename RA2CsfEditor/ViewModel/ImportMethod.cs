using RA2CsfEditor.Model;
using System;
using System.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace RA2CsfEditor.ViewModel
{
    public partial class MainVM
    {
        // 打开文件(异步)
        public async Task ImportFromCsf(string path)
        {
            // 设置默认CSF文件位置
            _path = path;
            // 创建新的CSF对象
            ThisCSF = new CSF();
            // 调用 CSF 类中的异步方法
            await ThisCSF.LoadFromFile(path);
            // 更新
            Update();
        }
        // 从Json导入
        public void ImportFromJson(string path)
        {
            // 创建CSF对象
            var csf = new CSF();
            // 获取 Json文件Root Object
            var root = JsonValue.Parse(
                // 使用神奇正则将Json注释替换
                System.Text.RegularExpressions.Regex.Replace(
                    // Json字符串
                    System.IO.File.ReadAllText(path),
                    // Json注释正则
                    "\\/\\*.?\\*\\/|\\/\\/.?$",
                    // 要替换的字符串
                    string.Empty))
                // 转换为JsonObject
                as JsonObject;
            // 从Root对象中尝试获取值 失败返回false
            if (!root.TryGetValue("Strings", out JsonValue jav) || // 标签数组
                !root.TryGetValue("Version", out JsonValue ver) || // CSF 版本
                !root.TryGetValue("Language", out JsonValue lang) || // CSF 语言
                jav.JsonType != JsonType.Array)// 类型检查
                return;// 不通过检查
            // 创建CSF文件头
            csf.MakeHeader(ver, lang);
            // 类型转换
            JsonArray labelArray = jav as JsonArray;
            MaxStatus = labelArray.Count;
            for (Status = 0; Status < MaxStatus; Status++)
            {
                // 尝试取出字符串
                if (!(labelArray[Status] as JsonObject).TryGetValue("Label", out JsonValue label) || // 获取标签名
                    !(labelArray[Status] as JsonObject).TryGetValue("Value", out JsonValue tmpValues)) // 获取标签值数组
                    continue;// 检查不通过
                // 尝试取出标签额外值
                (labelArray[Status] as JsonObject).TryGetValue("ExtraValue", out JsonValue evalue);
                // 向CSF对象添加标签
                csf.AddLabel(
                    // 创建新的标签对象
                    new Label(
                        // 标签名
                        label,
                        // 标签值IEnumerable
                        Value.ConvertArray(
                            // LINQ : 类型转换
                            from value in tmpValues as JsonArray select (string)value),
                        // 额外值
                        evalue));
            }
            // 赋值
            ThisCSF = csf;
            // 更新
            Update();
        }
        // 从XML导入CSF
        public void ImportFromXml(string path)
        {

            // 创建一个XML对象
            var xml = new XmlDocument();
            // 读取文件
            xml.Load(
                // 创建一个XML读取
                XmlReader.Create(
                    // 从这里读取
                    path as string,
                    // 创建一个读取设置对象
                    new XmlReaderSettings
                    {
                            //忽略文档里面的注释
                            IgnoreComments = true
                    }));
            // 获取Root节点
            var root = xml.DocumentElement;
            // 创建 CSF 对象并返回
            ThisCSF = new CSF(
                 // CSF 版本
                 Convert.ToInt32(root.GetAttribute("Version")),
                 // CSF 文件语言
                 Convert.ToInt32(root.GetAttribute("Language")),
                 // LINQ : 获取标签IEnumerable
                 from XmlNode label in root.ChildNodes
                     // 创建标签对象
                     select new Label(
                                  // 标签名
                                  (label as XmlElement).GetAttribute("Name"),
                                  // 标签值列表 Convert IEnumerable<string> To IEnumerable<Value>
                                  Value.ConvertArray(
                                      // IEnumerable<string>
                                      from XmlNode value in (label as XmlElement).ChildNodes
                                          // 获取 Value 字符串
                                          select (value as XmlElement).GetAttribute("Content")),
                                  // 获取额外值
                                  (label as XmlElement).GetAttribute("ExtraValue")));
            // View更新
            Update();
        }

    }
}
