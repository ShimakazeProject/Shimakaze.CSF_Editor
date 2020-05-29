using Shimakaze.ToolKit.CSF.Kernel.Extension;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DWORD = System.Int32;

namespace Shimakaze.ToolKit.CSF.Kernel
{
    public struct Head
    {
        public string Flag;
        public DWORD Version;
        public DWORD LabelCount;
        public DWORD StringCount;
        public DWORD Unknown;
        public DWORD Language;
        private Head(string flag, DWORD version, DWORD lc, DWORD sc, DWORD unknown, DWORD lang)
        {
            Flag = flag;
            Version = version;
            LabelCount = lc;
            StringCount = sc;
            Unknown = unknown;
            Language = lang;
        }
        public static Head CreateHead(DWORD Version, DWORD Language) => new Head(Constants.CSF_FLAG, Version, 0, 0, 0, Language);

        public static async Task<Head> Parse(Stream stream) =>
            new Head(
                Encoding.ASCII.GetString(await stream.ReadAsync(4)),
                BitConverter.ToInt32(await stream.ReadAsync(4), 0),
                BitConverter.ToInt32(await stream.ReadAsync(4), 0),
                BitConverter.ToInt32(await stream.ReadAsync(4), 0),
                BitConverter.ToInt32(await stream.ReadAsync(4), 0),
                BitConverter.ToInt32(await stream.ReadAsync(4), 0)
            );

        public static class Constants
        {
            public const string CSF_FLAG = " FSC";// 标准CSF文件标识符
            public const DWORD CSF_VERSION_2 = 2;
            public const DWORD CSF_VERSION_3 = 3;
        }
        public static class Languages
        {
            public const DWORD en_US = 0;
            public const DWORD en_UK = 1;
            public const DWORD de = 2;
            public const DWORD fr = 3;
            public const DWORD es = 4;
            public const DWORD it = 5;
            public const DWORD ja = 6;
            public const DWORD Jabberwockie = 7;// 不明语言
            public const DWORD ko = 8;
            public const DWORD zh = 9;
            public const DWORD Auto = -1;// Ares 某版本开始的特有的语言
        }
    }
}
