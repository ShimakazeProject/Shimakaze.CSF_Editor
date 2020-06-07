using Shimakaze.ToolKit.CSF.Kernel.Extension;

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Shimakaze.ToolKit.CSF.Kernel
{
    public sealed class CsfHeadStruct
    {
        public int Version { get; set; } = CSF_VERSION_3;
        public int LabelCount { get; set; } = 0;
        public int StringCount { get; set; } = 0;
        public int Unknown { get; set; } = 0;
        public int Language { get; set; } = 0;
        /// <summary>
        /// 空的构造方法
        /// </summary>
        public CsfHeadStruct()
        {
        }
        public CsfHeadStruct(int version, int lc, int sc, int unknown, int lang)
        {
            Version = version;
            LabelCount = lc;
            StringCount = sc;
            Unknown = unknown;
            Language = lang;
        }

        public static async Task<CsfHeadStruct> ParseAsync(Stream stream)
        {
            string flag = Encoding.ASCII.GetString(await stream.ReadAsync(4));
            if (flag.Equals(CSF_FLAG)) throw new FormatException("Unknown File Format: Unknown Header");
            return new CsfHeadStruct(
                BitConverter.ToInt32(await stream.ReadAsync(4), 0),
                BitConverter.ToInt32(await stream.ReadAsync(4), 0),
                BitConverter.ToInt32(await stream.ReadAsync(4), 0),
                BitConverter.ToInt32(await stream.ReadAsync(4), 0),
                BitConverter.ToInt32(await stream.ReadAsync(4), 0));
        }

        public const string CSF_FLAG = " FSC";// 标准CSF文件标识符
        public const int CSF_VERSION_2 = 2;
        public const int CSF_VERSION_3 = 3;

        public const int Languages_en_US = 0;
        public const int Languages_en_UK = 1;
        public const int Languages_de = 2;
        public const int Languages_fr = 3;
        public const int Languages_es = 4;
        public const int Languages_it = 5;
        public const int Languages_ja = 6;
        public const int Languages_Jabberwockie = 7;// 不明语言
        public const int Languages_ko = 8;
        public const int Languages_zh = 9;
        public const int Languages_Auto = -1;// Ares 某版本开始的特有的语言
    }
}
