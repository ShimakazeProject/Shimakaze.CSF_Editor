using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSF.Core
{
    public abstract class CsfValue
    {

        public abstract string ValueTag { get; }
        public abstract int ValueLength { get; }
        public abstract string ValueString { get; set; }
        public abstract int? ExtraLength { get; }
        public abstract string ExtraString { get; set; }

        /// <summary>
        /// 编/解码
        /// </summary>
        /// <param name="ValueLength">长度</param>
        /// <param name="ValueData">内容</param>
        /// <returns>编/解码后的数组</returns>
        public static byte[] Decoding(int ValueLength, byte[] ValueData)
        {
            int ValueDataLength = ValueLength << 1;
            for (int i = 0; i < ValueDataLength; i++)
                ValueData[i] = (byte)~ValueData[i];
            return ValueData;
        }
    }
    public class ValueMaker : CsfValue
    {
        private readonly string valueTag;
        private readonly int valueLength;
        private readonly int? extraLength;
        private string valueString;
        private string extraString;

        public override string ValueTag => valueTag;
        public override int ValueLength => valueLength;
        public override string ValueString { get => valueString; set => valueString = value; }
        public override int? ExtraLength => extraLength;
        public override string ExtraString { get => extraString; set => extraString = value; }
        public ValueMaker(string valueTag, int valueLength, string valueString, int? extraLength = null, string extraString = null)
        {
            this.valueTag = valueTag;
            this.valueLength = valueLength;
            this.valueString = valueString;
            this.extraLength = extraLength;
            this.extraString = extraString;
        }
    }
}
