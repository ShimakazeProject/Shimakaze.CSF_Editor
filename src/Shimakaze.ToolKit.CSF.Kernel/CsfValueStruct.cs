using Shimakaze.ToolKit.CSF.Kernel.Extension;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Shimakaze.ToolKit.CSF.Kernel
{
    public sealed class CsfValueStruct : INotifyPropertyChanged
    {
        public const string CSF_VALUE_STR = " RTS";
        public const string CSF_VALUE_WSTR = "WRTS";
        private string extra = string.Empty;
        private string content = string.Empty;
        private bool isWstr = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsWstr
        {
            get => isWstr; set
            {
                isWstr = value;
                OnPropertyChanged(nameof(IsWstr));
            }
        }
        public string Content
        {
            get => content; set
            {
                content = value;
                OnPropertyChanged(nameof(Content));
            }
        }
        public string Extra
        {
            get => extra; set
            {
                extra = value;
                OnPropertyChanged(nameof(Extra));
            }
        }
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// 异步解析
        /// </summary>
        public static async Task<CsfValueStruct> ParseAsync(Stream stream)
        {
            string flag, content, extra;
            int length, extraLength;
            var t1 = stream.ReadAsync(4);
            var t2 = stream.ReadAsync(4);

            var value = new CsfValueStruct();
            // 字符串标记
            flag = Encoding.ASCII.GetString(await t1);
            // 字符串主要内容长度
            length = BitConverter.ToInt32(await t2, 0);
            // 字符串主要内容
            value.Content = content = Encoding.Unicode.GetString(Decoding(await stream.ReadAsync(length << 1)));
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
        /// 同步解析
        /// </summary>
        public static CsfValueStruct Parse(Stream stream)
        {
            var value = new CsfValueStruct();
            string flag, content, extra;
            int length, extraLength;
            // 字符串标记
            flag = Encoding.ASCII.GetString(stream.Read(4));
            // 字符串主要内容长度
            length = BitConverter.ToInt32(stream.Read(4), 0);
            // 字符串主要内容
            value.Content = content = Encoding.Unicode.GetString(Decoding(stream.Read(length << 1)));
            // 判断是否包含额外内容
            if (flag.Equals(CSF_VALUE_WSTR))// 存在额外内容
            {
                value.IsWstr = true;
                // 额外内容长度
                extraLength = BitConverter.ToInt32(stream.Read(4), 0);
                // 额外内容
                value.Extra = extra = Encoding.ASCII.GetString(stream.Read(extraLength));
            }
            return value;
        }


        /// <summary>
        /// 值字符串 编/解码<br/>
        /// CSF文档中的Unicode编码内容都是按位异或的<br/>
        /// 这个方法使用for循环实现
        /// </summary>
        /// <param name="ValueLength">长度</param>
        /// <param name="ValueData">内容</param>
        /// <returns>编/解码后的数组</returns>
        [Obsolete("方法会自动判断长度")]
        public static byte[] Decoding(int ValueLength, byte[] ValueData)
        {
            int ValueDataLength = ValueLength << 1;
            for (int i = 0; i < ValueDataLength; ++i)
                ValueData[i] = (byte)~ValueData[i];
            return ValueData;
        }
        /// <summary>
        /// 值字符串 编/解码<br/>
        /// CSF文档中的Unicode编码内容都是按位异或的<br/>
        /// 这个方法使用for循环实现
        /// </summary>
        /// <param name="ValueData">内容</param>
        /// <returns>编/解码后的数组</returns>
        public static byte[] Decoding(byte[] ValueData)
        {
            for (int i = 0; i < ValueData.Length; ++i)
                ValueData[i] = (byte)~ValueData[i];
            return ValueData;
        }
        /// <summary>
        /// 值字符串 编/解码<br/>
        /// CSF文档中的Unicode编码内容都是按位异或的<br/>
        /// 这个方法使用<see cref="System.Linq.Enumerable"/>的扩展方法实现
        /// </summary>
        /// <param name="ValueData">内容</param>
        /// <returns></returns>
        public static IEnumerable<byte> Decoding(IEnumerable<byte> ValueData) => ValueData.Select(i => (byte)~i);
    }
}
