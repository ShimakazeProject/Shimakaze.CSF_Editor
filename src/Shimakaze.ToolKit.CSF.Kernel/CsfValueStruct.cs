using Shimakaze.ToolKit.CSF.Kernel.Extension;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace Shimakaze.ToolKit.CSF.Kernel
{
    public sealed class CsfValueStruct
    {
        public const string CSF_VALUE_STR = " RTS";
        public const string CSF_VALUE_WSTR = "WRTS";

        public bool IsWstr { get; set; } = false;
        public string Content { get; set; } = string.Empty;
        public string Extra { get; set; } = string.Empty;

        public static async Task<CsfValueStruct> Parse(Stream stream)
        {
            var value = new CsfValueStruct();
            string flag, content, extra;
            int length, extraLength;
            // 字符串标记
            flag = Encoding.ASCII.GetString(await stream.ReadAsync(4));
            // 字符串主要内容长度
            length = BitConverter.ToInt32(await stream.ReadAsync(4), 0);
            // 字符串主要内容
            value.Content = content = Encoding.Unicode.GetString(Decoding(length, await stream.ReadAsync(length)));
            // 判断是否包含额外内容
            if (flag.Equals(CSF_VALUE_WSTR))// 存在额外内容
            {
                value.IsWstr = true;
                // 额外内容长度
                extraLength = BitConverter.ToInt32(await stream.ReadAsync(4), 0);
                // 额外内容
                value.Extra = extra = Encoding.ASCII.GetString(await stream.ReadAsync(extraLength));
            }
            return value;
        }

        /// <summary>
        /// 值字符串 编/解码
        /// </summary>
        /// <param name="ValueLength">长度</param>
        /// <param name="ValueData">内容</param>
        /// <returns>编/解码后的数组</returns>
        public static byte[] Decoding(int ValueLength, byte[] ValueData)
        {
            int ValueDataLength = ValueLength << 1;
            for (int i = 0; i < ValueDataLength; ++i)
                ValueData[i] = (byte)~ValueData[i];
            return ValueData;
        }
    }
}
