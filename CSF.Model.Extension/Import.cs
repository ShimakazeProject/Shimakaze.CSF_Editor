using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace CSF.Model.Extension
{
    public static class Import
    {
        public static async Task<List<Type>> Add(this List<Type> types, Label label,List<Model.Value> values)
        {
            bool finished = false;
            await Task.Run(() =>
            {
                for (int i = 0; i < types.Count; i++)
                {
                    if (types[i].Name == label.GetTypeName())
                    {
                        types[i].Add(label, values);
                        finished = true;
                        return;
                    }
                }
            });
            if (!finished) types.Add(new Type(label, values));

            return types;

        }
        public static async Task<List<Type>> FromFile(File File)
        {
            var types = new List<Type>();
            foreach (var label in File.Labels)
            {
                types = await types.Add(label.Key,label.Value);
            }
            return types;
        }
        public static async Task<List<Type>> FromCSF(string path)
        {
            var bytes = System.IO.File.ReadAllBytes(path);            
            var file = new File();
            await file.LoadFileAsync(bytes);

            return await FromFile(file);// 这里....要想办法分出来个线程
            //throw new Exception();
        }

        // 从Json导入
        public static async Task<Type[]> FromJson(string path)
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

            if (!root.TryGetValue("JsonVersion",out JsonValue jsonVersion))
            {
                int version = jsonVersion;
                switch (version)
                {
                    //case 1:
                    //    return await FromFile(Json.Import.V1(root));
                }
            }
            throw new Exception();
        }
        // 从XML导入CSF
        public static async Task<Type[]> FromXml(string path)
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
                //case 1:
                //    return await FromFile(Xml.Import.V1(root));

            }
            throw new Exception();
        }
    }
}
