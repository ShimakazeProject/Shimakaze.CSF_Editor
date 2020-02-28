using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.Plugin.Convert
{
    public class Zh_HantToHans : ConvertPlugin
    {
        public override string Name => "繁体 => 简体";

        public override string Description => "将文档文字从繁体转换为简体";

        public override string ExecuteName => nameof(Hant2HansFile);

        public override string SingleName => Name;

        public override string SingleDescription => "将当前选中项的文字从繁体转换为简体";

        public override string SingleExecuteName => nameof(Hant2Hans);
        public static Model.File Hant2HansFile(Model.File file)
        {
            foreach (Model.Label label in file.Labels)
                foreach (Model.Value value in label.LabelValues)
                    value.ValueString = Hant2Hans(value.ValueString);
            return file;
        }
        public static string Hant2Hans(string s)
        {
            return ChineseConverter.Convert(s, ChineseConversionDirection.TraditionalToSimplified);//转简体
        }
    }
}
