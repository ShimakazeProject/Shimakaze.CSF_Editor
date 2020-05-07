using Shimakaze.CSF.Kernel.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DWORD = System.Int32;

namespace Shimakaze.CSF.Kernel
{
    public class Value
    {
        public const string CSF_VALUE_STR = " RTS";
        public const string CSF_VALUE_WSTR = "WRTS";

        public string Flag;
        public DWORD ValueLength;
        public string ValueContent;
        public DWORD? ExtraLength;
        public string ExtraContent;

        public static async Task<Value> Parse(Stream stream)
        {
            int length;
            var value = new Value
            {
                Flag = Encoding.ASCII.GetString(await stream.ReadAsync(4)),
                ValueLength = length = BitConverter.ToInt32(await stream.ReadAsync(4), 0),
                ValueContent = Encoding.Unicode.GetString(CSFFile.Decoding(length, await stream.ReadAsync(length)))
            };

            if (value.Flag.Equals(CSF_VALUE_WSTR))            
                value.ExtraContent = Encoding.ASCII.GetString(await stream.ReadAsync((DWORD)(value.ExtraLength = BitConverter.ToInt32(await stream.ReadAsync(4), 0))));
            
            return value;
        }
    }
}
