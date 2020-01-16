using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frg2089.CSF.Core.Helper
{
    public class LabelHelper : ILabel
    {
        public string LabelTag { get; private set; }

        public int StringCount { get; private set; }

        public int NameLength { get; private set; }

        public string LabelName { get; set; }

        public IEnumerable<IValue> Values { get; set; }

        public int Length => 0x0C + NameLength + (from value in Values select value.Length).Sum();

        private LabelHelper(string labelTag, int stringCount, int nameLength, string labelName, IEnumerable<IValue> values)
        {
            LabelTag = labelTag;
            StringCount = stringCount;
            NameLength = nameLength;
            LabelName = labelName;
            Values = values;
        }

        public static ILabel CreateLabel(IEnumerable<byte> label)
        {
            var tag = Encoding.ASCII.GetString(label.Skip(0x00).Take(4).ToArray());
            var stringCount = BitConverter.ToInt32(label.Skip(0x04).Take(4).ToArray(), 0);
            var nameLength = BitConverter.ToInt32(label.Skip(0x08).Take(4).ToArray(), 0);
            var labelName = Encoding.ASCII.GetString(label.Skip(0x0C).Take(nameLength).ToArray());
            var values = new IValue[stringCount];
            int start = 0x0C + nameLength;
            for (int i = 0; i < stringCount; i++)
            {
                var value = ValueHelper.CreateValue(label.Skip(start));
                start += value.Length;
                values[i] = value;
            }
            return new LabelHelper(tag, stringCount, nameLength, labelName, values);
        }
    }
}
