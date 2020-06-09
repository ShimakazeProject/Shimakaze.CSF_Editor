using Shimakaze.ToolKit.CSF.Kernel.Extension;

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shimakaze.ToolKit.CSF.Kernel
{
    public sealed class CsfHeadStruct : INotifyPropertyChanged
    {
        public int Version
        {
            get => version; set
            {
                version = value;
                OnPropertyChanged(nameof(Version));
            }
        }
        public int LabelCount
        {
            get => labelCount; set
            {
                labelCount = value;
                OnPropertyChanged(nameof(LabelCount));
            }
        }
        public int StringCount
        {
            get => stringCount; set
            {
                stringCount = value;
                OnPropertyChanged(nameof(StringCount));
            }
        }
        public int Unknown
        {
            get => unknown; set
            {
                unknown = value;
                OnPropertyChanged(nameof(Unknown));
            }
        }
        public int Language
        {
            get => language; set
            {
                language = value;
                OnPropertyChanged(nameof(Language));
            }
        }
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
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public static async Task<CsfHeadStruct> ParseAsync(Stream stream)
        {
            string flag = Encoding.ASCII.GetString(await stream.ReadAsync(4));
            if (!flag.Equals(CSF_FLAG)) throw new FormatException("Unknown File Format: Unknown Header");
            var ver = BitConverter.ToInt32(await stream.ReadAsync(4), 0);
            var lc = BitConverter.ToInt32(await stream.ReadAsync(4), 0);
            var sc = BitConverter.ToInt32(await stream.ReadAsync(4), 0);
            var uknow = BitConverter.ToInt32(await stream.ReadAsync(4), 0);
            var lang = BitConverter.ToInt32(await stream.ReadAsync(4), 0);
            return new CsfHeadStruct(ver,lc,sc,uknow,lang);
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
        private int language = 0;
        private int unknown = 0;
        private int stringCount = 0;
        private int labelCount = 0;
        private int version = CSF_VERSION_3;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
