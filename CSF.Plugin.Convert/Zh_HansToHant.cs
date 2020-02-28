using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.Plugin.Convert
{
    public class Zh_HansToHant : ConvertPlugin
    {
        public override string Name => "简体 => 繁体";
        
        public override string Description => "将文档文字从简体转换为繁体";
                
        public override string ExecuteName => nameof(Hans2HantFile);

        public override string SingleName => Name;

        public override string SingleDescription => "将当前选中项的文字从简体转换为繁体";

        public override string SingleExecuteName => nameof(Hans2Hant);

        public static Model.File Hans2HantFile(Model.File file)
        {
            foreach (Model.Label label in file.Labels)
                foreach (Model.Value value in label.LabelValues)
                    value.ValueString = Hans2Hant(value.ValueString);
            return file;
        }
        public static string Hans2Hant(string s)
        {
            return ChineseConverter.Convert(s, ChineseConversionDirection.SimplifiedToTraditional);//转繁体
        }
    }
}
