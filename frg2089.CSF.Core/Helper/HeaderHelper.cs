using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frg2089.CSF.Core.Helper
{
    public class HeaderHelper : IHeader
    {
        public string Flag { get; private set; }

        public int Version { get; private set; }

        public int LabelCount { get; private set; }

        public int StringCount { get; private set; }

        public IEnumerable<byte> Unknow { get; private set; }

        public int Language { get; private set; }

        private HeaderHelper(string flag,int version,int labelCount,int stringCount, IEnumerable<byte> unknow,int language)
        {
            Flag = flag;
            Version = version;
            LabelCount = labelCount;
            StringCount = stringCount;
            Unknow = unknow;
            Language = language;
        }

        public static IHeader CreateHeader(IEnumerable<byte> bytes)
        {
            if (bytes.Count()==24)
            {
                return new HeaderHelper(Encoding.ASCII.GetString(bytes.Skip(0x00).Take(4).ToArray()),
                                        BitConverter.ToInt32(bytes.Skip(0x04).Take(4).ToArray(), 0),
                                        BitConverter.ToInt32(bytes.Skip(0x08).Take(4).ToArray(), 0),
                                        BitConverter.ToInt32(bytes.Skip(0x0C).Take(4).ToArray(), 0),
                                        bytes.Skip(0x10).Take(4),
                                        BitConverter.ToInt32(bytes.Skip(0x14).Take(4).ToArray(), 0));
            }
            else
            {
                throw new FormatException("It's Not A Header Bytes");
            }
        }
    }
}
