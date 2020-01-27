using System;
using System.Json;
using System.Linq;
using System.Xml;

namespace CSF.Model.Extension
{
    public static class Import
    {
        public static void FromCSF(this Type type, Core.IFile File)
        {
            type = new Type();
            foreach (var label in File.Labels)
                type.Add(label);
        }
        public static void FromCSF(this Type type, string path) => type.FromCSF(Core.Helper.FileHelper.CreateFile(System.IO.File.ReadAllBytes(path)));

        // 从Json导入
        public static void FromJson(this Type type, string path)
        {
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
            var a=root["JsonVersion"];


            if (!root.TryGetValue("JsonVersion",out JsonValue jsonVersion))
            {
                int version = jsonVersion;
                switch (version)
                {
                    case 1:
                        type.FromCSF(Json.Import.V1(root));
                        break;
                    default:
                        break;
                }
            }            
        }
        // 从XML导入CSF
        public static void FromXml(this Type type, string path)
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
                    //忽略文档里面的注释
                    { IgnoreComments = true }));
            // 获取Root节点
            var root = xml.DocumentElement;

            switch (Convert.ToInt32(root.GetAttribute("XmlVersion")))
            {
                case 1:
                    type.FromCSF(Xml.Import.V1(root));
                    break;
                default:
                    break;

            }
        }
    }
}
