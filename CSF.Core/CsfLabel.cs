using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSF.Core
{
    public abstract class CsfLabel
    {
        public abstract string LabelTag { get; }

        public abstract int StringCount { get; }

        public abstract int NameLength { get; }

        public abstract string LabelName { get; set; }

        public abstract CsfValue[] Values { get; set; }

        public static CsfValue GetValue(ByteReader byteReader)
        {

            var strTag = Encoding.ASCII.GetString(byteReader.Read(4));
            var strLenght = BitConverter.ToInt32(byteReader.Read(4), 0);
            var strValue = Encoding.Unicode.GetString(
                CsfValue.Decoding(strLenght, byteReader.Read(strLenght * 2)));
            if (strTag.Equals("WRTS"))
            {
                var strELenght = BitConverter.ToInt32(byteReader.Read(4), 0);
                var strEValue = Encoding.ASCII.GetString(byteReader.Read(strELenght));
                return GetValue(strTag, strLenght, strValue, strELenght, strEValue);
            }
            else //if (strTag.Equals(" RTS"))
                return GetValue(strTag, strLenght, strValue);
        }
        public static CsfValue GetValue(string valueTag, int valueLength, string valueString, int? extraLength = null, string extraString = null)
            => new ValueMaker(valueTag, valueLength, valueString, extraLength, extraString);

        public static CsfLabel GetLabel(Stream stream)
        {
            using (var br = new ByteReader(stream))
            {
                var tag = Encoding.ASCII.GetString(br.Read(4));
                var count = BitConverter.ToInt32(br.Read(4), 0);
                var length = BitConverter.ToInt32(br.Read(4), 0);
                var name = Encoding.ASCII.GetString(br.Read(4));
                var valueArray = new CsfValue[length];
                for (int i = 0; i < count; i++)
                    valueArray[i] = GetValue(br);

                return GetLabel(tag, count, length, name, valueArray);
            }

        }
        public static CsfLabel GetLabel(byte[] labelTag, byte[] stringCount, byte[] nameLength, byte[] labelName, byte[] values)
        {
            var tag = Encoding.ASCII.GetString(labelTag);
            var count = BitConverter.ToInt32(stringCount, 0);
            var length = BitConverter.ToInt32(nameLength, 0);
            var name = Encoding.ASCII.GetString(labelName);
            var valueArray = new CsfValue[length];
            using (var br = new ByteReader(new MemoryStream(values)))
                for (int i = 0; i < count; i++)
                    valueArray[i] = GetValue(br);

            return GetLabel(tag, count, length, name, valueArray);
        }
        public static CsfLabel GetLabel(string labelTag, int stringCount, int nameLength, string labelName, CsfValue[] values)
            => new LabelMaker(labelTag, stringCount, nameLength, labelName, values);
    }
    public class LabelMaker : CsfLabel
    {
        private readonly string labelTag;
        private readonly int stringCount;
        private readonly int nameLength;
        private string labelName;
        private CsfValue[] values;

        public override string LabelTag => labelTag;
        public override int StringCount => stringCount;
        public override int NameLength => nameLength;
        public override string LabelName { get => labelName; set => labelName = value; }
        public override CsfValue[] Values { get => values; set => values = value; }
        public LabelMaker(string labelTag, int stringCount, int nameLength, string labelName, CsfValue[] values)
        {
            this.labelTag = labelTag;
            this.stringCount = stringCount;
            this.nameLength = nameLength;
            this.labelName = labelName;
            this.values = values;
        }
    }
}
