using Shimakaze.ToolKit.CSF.Kernel.Extension;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using DWORD = System.Int32;

namespace Shimakaze.ToolKit.CSF.Kernel
{
    public struct Value
    {
        public const string CSF_VALUE_STR = " RTS";
        public const string CSF_VALUE_WSTR = "WRTS";

        public string Flag;
        public DWORD Length;
        public string Content;
        public DWORD? ExtraLength;
        public string ExtraContent;

        public static async Task<Value> Parse(Stream stream)
        {
            int length;
            var value = new Value
            {
                Flag = Encoding.ASCII.GetString(await stream.ReadAsync(4)),
                Length = length = BitConverter.ToInt32(await stream.ReadAsync(4), 0),
                Content = Encoding.Unicode.GetString(CSFFile.Decoding(length, await stream.ReadAsync(length)))
            };

            if (value.Flag.Equals(CSF_VALUE_WSTR))
                value.ExtraContent = Encoding.ASCII.GetString(await stream.ReadAsync((DWORD)(value.ExtraLength = BitConverter.ToInt32(await stream.ReadAsync(4), 0))));

            return value;
        }

        public override bool Equals(object obj) =>
            (obj is Value value)                                // 判断类型
            && (ReferenceEquals(this, value)                    // 判断引用
            || ((this.Flag.Equals(value.Flag)                   // 标记是否相同
            || this.Length.Equals(value.Length)                 // 长度是否相同
            || this.Content.Equals(value.Content))              // 内容是否相同
            && (!this.Flag.Equals(Value.CSF_VALUE_WSTR)         // 是否有额外值
            || this.ExtraLength.Equals(value.ExtraLength)       // 额外值长度是否相同
            || this.ExtraContent.Equals(value.ExtraContent)))); // 额外内容是否相同


        public override int GetHashCode() => (Content, ExtraContent).GetHashCode();

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
