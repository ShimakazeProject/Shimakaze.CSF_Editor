using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSF.Model
{
    public class File
    {
        /// <summary>
        /// 文件标记 始终为" FSC"
        /// </summary>
        public string Flag { get;  set; }
        /// <summary>
        /// 文件版本
        /// </summary>
        public int Version { get;  set; }
        /// <summary>
        /// 标签数
        /// </summary>
        public int LabelCount { get;  set; }
        /// <summary>
        /// 字符串数
        /// </summary>
        public int StringCount { get;  set; }
        /// <summary>
        /// 额外信息 4字节
        /// </summary>
        public byte[] Unknow { get;  set; }
        /// <summary>
        /// 语言信息
        /// </summary>
        public int Language { get;  set; }
        /// <summary>
        /// 标签信息
        /// </summary>
        public Label[] Labels { get; set; }
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath">文件位置</param>
        public void LoadFromFile(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                LoadFromStream(fs);
            }
        }
        /// <summary>
        /// 从流中读取CSF文件
        /// </summary>
        /// <param name="stream">流</param>
        public void LoadFromStream(Stream stream)
        {
            // Header 
            Flag = Encoding.ASCII.GetString(stream.Read(4));
            Version = BitConverter.ToInt32(stream.Read(4), 0);
            LabelCount = BitConverter.ToInt32(stream.Read(4), 0);
            StringCount = BitConverter.ToInt32(stream.Read(4), 0);
            Unknow = stream.Read(4);
            Language = BitConverter.ToInt32(stream.Read(4), 0);
            // Labels
            Labels = new Label[LabelCount];
            for (int i = 0; i < LabelCount; i++)
            {
                var labelFlag = Encoding.ASCII.GetString(stream.Read(4));
                var labelCount = BitConverter.ToInt32(stream.Read(4), 0);
                var labelLength = BitConverter.ToInt32(stream.Read(4), 0);
                var labelName = Encoding.ASCII.GetString(stream.Read(labelLength));
                var labelValues = new Value[labelCount];
                for (int j = 0; j < labelCount; j++)
                {
                    var valueFlag = Encoding.ASCII.GetString(stream.Read(4));
                    var valueLength = BitConverter.ToInt32(stream.Read(4), 0);
                    var valueString = Encoding.Unicode.GetString(Decoding(valueLength, stream.Read(valueLength * 2)));
                    if (valueFlag == "WRTS")
                    {
                        var extraLength = BitConverter.ToInt32(stream.Read(4), 0);
                        var extraString = Encoding.ASCII.GetString(stream.Read(extraLength));
                        labelValues[j] = new Value(valueFlag, valueLength, valueString, extraLength, extraString);
                    }
                    else if (valueFlag == " RTS")
                    {
                        labelValues[j] = new Value(valueFlag, valueLength, valueString);
                    }
                    else throw new Exception("喵喵喵???");
                }
                Labels[i] = new Label(labelFlag, labelCount, labelLength, labelName, labelValues);
            }
        }
        /// <summary>
        /// 值字符串 编/解码
        /// </summary>
        /// <param name="ValueLength">长度</param>
        /// <param name="ValueData">内容</param>
        /// <returns>编/解码后的数组</returns>
        internal static byte[] Decoding(int ValueLength, byte[] ValueData)
        {
            int ValueDataLength = ValueLength << 1;
            for (int i = 0; i < ValueDataLength; ++i)
            {
                ValueData[i] = (byte)~ValueData[i];
            }
            return ValueData;
        }

        #region Indexer
        public virtual Label this[int index]
        {
            get => Labels[index];
            set => Labels[index] = value;
        }
        public virtual Label this[string name]
        {
            get
            {
                for (int i = 0; i < Labels.Length; i++)
                {
                    if (Labels[i].LabelName.Equals(name))
                    {
                        return Labels[i];
                    }
                }
                return null;
            }
            set
            {
                for (int i = 0; i < Labels.Length; i++)
                {
                    if (Labels[i].LabelName.Equals(name))
                    {
                        Labels[i] = value;
                    }
                }
            }
        }
        #endregion
    }
}
