using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;

namespace CSF.Convert
{
    public static class Zh
    {
        public static string Hans2Hant(string s)
        {
            return ChineseConverter.Convert(s, ChineseConversionDirection.SimplifiedToTraditional);//转繁体
        }
        public static string Hant2Hans(string s)
        {
            return ChineseConverter.Convert(s, ChineseConversionDirection.TraditionalToSimplified);//转简体
        }
        public static string Hans2HantName => "简体 => 繁体";
        public static string Hant2HansName => "繁体 => 简体";
        public static Model.File Hans2HantFile(Model.File file)
        {
            foreach (Model.Label label in file.Labels)
                foreach (Model.Value value in label.LabelValues)
                    value.ValueString = Hans2Hant(value.ValueString);
            return file;
        }
        public static Model.File Hant2HansFile(Model.File file)
        {
            foreach (Model.Label label in file.Labels)
                foreach (Model.Value value in label.LabelValues)
                    value.ValueString = Hant2Hans(value.ValueString);
            return file;
        }
    }
}
