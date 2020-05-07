using Shimakaze.CSF.Kernel.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DWORD = System.Int32;

namespace Shimakaze.CSF.Kernel
{
    /// <summary>
    /// CSF文件的标签结构
    /// </summary>
    public class Label
    {
        public const string CSF_LABEL_FLAG = " LBL";
        public string Flag;
        public DWORD StringCount;
        public DWORD NameLength;
        public string Name;
        public Value[] Values;

        public static async Task<Label> Parse(Stream stream)
        {
            var values = new List<Value>();
            DWORD nameLength;
            var lbl = new Label
            {
                Flag = Encoding.ASCII.GetString(await stream.ReadAsync(4)),
                StringCount = BitConverter.ToInt32(await stream.ReadAsync(4), 0),
                NameLength = nameLength = BitConverter.ToInt32(await stream.ReadAsync(4), 0),
                Name = Encoding.ASCII.GetString(await stream.ReadAsync(nameLength))
            };

            for (int i = 0; i < lbl.StringCount; i++)
                values.Add(await Value.Parse(stream));
            lbl.Values = values.ToArray();
            return lbl;
        }
    }
}
