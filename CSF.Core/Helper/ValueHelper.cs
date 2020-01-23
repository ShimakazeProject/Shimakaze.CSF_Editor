using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Core.Helper
{
    public class ValueHelper : IValue
    {

        private ValueHelper(string valueTag,int valueLength,string valueString,int? extraLength,string extraString)
        {
            ValueTag = valueTag;
            ValueLength = valueLength;
            ValueString = valueString;
            ExtraLength = extraLength;
            ExtraString = extraString;
        }

        public string ValueTag { get; private set; }

        public int ValueLength { get; private set; }

        public string ValueString { get; set; }

        public int? ExtraLength { get; private set; }

        public string ExtraString { get; set; }

        public int Length => (ValueLength * 2) + 0x0C + ExtraLength ?? 0;

        public static IValue CreateValue(IEnumerable<byte> value)
        {
            var vlength = BitConverter.ToInt32(value.Skip(0x04).Take(4).ToArray(), 0);
            var elength = BitConverter.ToInt32(value.Skip(0x08 + vlength * 2).Take(4).ToArray(), 0);
            return new ValueHelper(Encoding.ASCII.GetString(value.Skip(0x00).Take(4).ToArray()),
                                   vlength,
                                   Encoding.Unicode.GetString(Decoding(vlength, value.Skip(0x08).Take(vlength * 2).ToArray())),
                                   elength,
                                   Encoding.ASCII.GetString(value.Skip(0x0C + (vlength * 2)).Take(elength).ToArray()));
        }
        /// <summary>
        /// 编/解码
        /// </summary>
        /// <param name="ValueLength">长度</param>
        /// <param name="ValueData">内容</param>
        /// <returns>编/解码后的数组</returns>
        private static byte[] Decoding(int ValueLength, byte[] ValueData)
        {
            int ValueDataLength = ValueLength << 1;
            for (int i = 0; i < ValueDataLength; ++i)
            {
                ValueData[i] = (byte)~ValueData[i];
            }
            return ValueData;
        }
    }
}
