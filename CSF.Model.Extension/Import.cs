using System;
using System.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace CSF.Model.Extension
{
    public static class Import
    {
        public static async Task<Type[]> Add(this Type[] types, Label label)
        {
            var type = new Type(label);
            bool finished = false;
            await Task.Run(() =>
            {
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].Name == type.Name)
                    {
                        types[i].Add(label);
                        finished = true;
                        return;
                    }
                }
            });
            if (finished) return types;
            else return types.Concat(new Type[] { type }).ToArray();

        }
        public static async Task<Type[]> FromFile(Core.IFile File)
        {
            var types = Array.Empty<Type>();
            foreach (var label in File.Labels)
            {
                types = await types.Add(new Label(label));
            }
            return types;
        }
        public static async Task<Type[]> FromCSF(string path)
        {
            var bytes = System.IO.File.ReadAllBytes(path);
            return await FromFile(await Task.Run(() => Core.Helper.FileHelper.CreateFile(bytes)));// 这里....要想办法分出来个线程
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
                    case 1:
                        return await FromFile(Json.Import.V1(root));
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
                case 1:
                    return await FromFile(Xml.Import.V1(root));
            }
            throw new Exception();
        }
    }
}
