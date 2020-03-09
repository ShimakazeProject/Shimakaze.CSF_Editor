using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSF.Model
{
    public class File : Header, IEnumerable
    {
        #region Internal Methods
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
                ValueData[i] = (byte)~ValueData[i];
            return ValueData;
        }
        #endregion Internal Methods

        #region Public Properties
        /// <summary>
        /// 标签信息
        /// </summary>
        public Label[] Labels { get; set; }
        #endregion Public Properties

        #region Public Methods
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath">文件位置</param>
        public virtual void LoadFromFile(string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            LoadFromStream(fs);
        }
        /// <summary>
        /// 从流中读取CSF文件
        /// </summary>
        /// <param name="stream">流</param>
        public virtual void LoadFromStream(Stream stream)
        {
            // Header 
            Flag = Encoding.ASCII.GetString(stream.Read(4));
            Version = BitConverter.ToInt32(stream.Read(4), 0);
            LabelCount = BitConverter.ToInt32(stream.Read(4), 0);
            StringCount = BitConverter.ToInt32(stream.Read(4), 0);
            Unknow = stream.Read(4);
            Language = (CsfLanguage)BitConverter.ToInt32(stream.Read(4), 0);
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
                    if (valueFlag == Value.WSTRING)
                    {
                        var extraLength = BitConverter.ToInt32(stream.Read(4), 0);
                        var extraString = Encoding.ASCII.GetString(stream.Read(extraLength));
                        labelValues[j] = new Value(valueFlag, valueLength, valueString, extraLength, extraString);
                    }
                    else if (valueFlag == Value.STRING)
                    {
                        labelValues[j] = new Value(valueFlag, valueLength, valueString);
                    }
                    else throw new Exception("喵喵喵???");
                }
                Labels[i] = new Label(labelFlag, labelCount, labelLength, labelName, labelValues);
            }
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath">路径</param>
        public virtual void SaveToFile(string filePath)
        {
            using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            SaveToStream(fs);
        }
        /// <summary>
        /// 序列化文件到流
        /// </summary>
        /// <param name="stream">流</param>
        public virtual void SaveToStream(Stream stream)
        {
            // Header
            ReCount();
            stream.WriteEx(Flag);
            stream.WriteEx(Version);
            stream.WriteEx(LabelCount);
            stream.WriteEx(StringCount);
            stream.WriteEx(Unknow);
            stream.WriteEx((int)Language);
            foreach (Label label in Labels)
            {
                stream.WriteEx(label.LabelFlag);
                stream.WriteEx(label.LabelStrCount);
                stream.WriteEx(label.LabelNameLength);
                stream.WriteEx(label.LabelName);
                foreach (var value in label.LabelValues)
                {
                    stream.WriteEx(value.ValueFlag);
                    stream.WriteEx(value.ValueLength);
                    stream.Write(Decoding(value.ValueLength, Encoding.Unicode.GetBytes(value.ValueString)));
                    if (value.ValueFlag == Value.WSTRING)
                    {
                        stream.WriteEx(value.ExtraLength);
                        stream.WriteEx(value.ExtraString);
                    }
                }
            }
        }
        public static File NewFile()
        {
            var file = new File();
            file.Flag = FlagStr;
            file.Version = 3;
            file.Unknow = new byte[4];
            file.Language = CsfLanguage.US;
            file.Add(new Model.Label("New:Label", new Model.Value[] { "新建CSF文件" }));
            file.ReCount();
            return file;
        }
        public virtual void ReCount()
        {
            LabelCount = Labels.Length;
            //StringCount = (from label in Labels select label.LabelStrCount).Sum();
            StringCount = (from label in Labels select label.LabelValues).Count();
        }
        public virtual IEnumerator GetEnumerator() => Labels.GetEnumerator();


        public virtual void Remove(Label label)
        {
            var list = Labels.ToList();
            list.Remove(label);
            Labels = list.ToArray();
        }
        public virtual void Add(Label label)
        {
            if (Labels is null)
                Labels = Array.Empty<Label>();
            var list = Labels.ToList();
            list.Add(label);
            Labels = list.ToArray();
        }
        #endregion Public Methods

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
                    if (Labels[i].LabelName.Equals(name))
                        return Labels[i];
                return null;
            }
            set
            {
                for (int i = 0; i < Labels.Length; i++)
                    if (Labels[i].LabelName.Equals(name))
                        Labels[i] = value;
            }
        }
        #endregion
    }
}
