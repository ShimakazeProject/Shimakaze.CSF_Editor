using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;

namespace CSF.Model.Extension.Json
{
    internal class Import
    {
        internal static Core.IFile V1(JsonObject jsonRoot)
        {
            var labelArray = new Label[0];

            // 文件检查
            if (!jsonRoot.TryGetValue("Header", out JsonValue header)||
                !jsonRoot.TryGetValue("Labels", out JsonValue labels))
                throw new Exception();

            // 获取文件头信息
            if (!(header as JsonObject).TryGetValue("Version", out JsonValue fileVersion) || // CSF 版本
                !(header as JsonObject).TryGetValue("Language", out JsonValue fileLanguage))// CSF 语言               
                throw new Exception();// 不通过检查

            if (labels.JsonType != JsonType.Array)
                throw new Exception();

            // 类型转换
            for (int i = 0; i < (labels as JsonArray).Count; i++)
            {
                // 尝试取出字符串
                if (!((labels as JsonArray)[i] as JsonObject).TryGetValue("Label", out JsonValue labelName) || // 获取标签名
                    !((labels as JsonArray)[i] as JsonObject).TryGetValue("Value", out JsonValue valueJsonObject)) // 获取标签值数组
                    continue;// 检查不通过
                var valueArray = new Model.Value[0];
                foreach (JsonObject value in valueJsonObject as JsonArray)
                {
                    if (!value.TryGetValue("String", out JsonValue str)) throw new Exception();
                    value.TryGetValue("Extra", out JsonValue extra);
                    valueArray.Concat(new Model.Value[] { new Model.Value(str, extra) });
                }
                labelArray.Concat(new Label[] { new Label(labelName, valueArray) });
            }

            return new Core.Helper.FileHelper(
                Core.Helper.HeaderHelper.MakeHeader(fileVersion, labelArray.Length, (from label in labelArray select label.StringCount).Sum(), fileLanguage),
                labelArray);
        }
    }
}