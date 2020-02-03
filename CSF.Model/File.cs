using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Model
{
    public class File
    {
        #region Field
        private string flag;
        private int version;
        private int labelCount;
        private int stringCount;
        private byte[] unknow;
        private int language;
        private Dictionary<Label, List<Value>> labels;
        #endregion

        #region Property
        /// <summary>
        /// 文件标记 始终为" FSC"
        /// </summary>
        public string Flag => flag;
        /// <summary>
        /// 文件版本
        /// </summary>
        public int Version => version;
        /// <summary>
        /// 标签数
        /// </summary>
        public int LabelCount => labelCount;
        /// <summary>
        /// 字符串数
        /// </summary>
        public int StringCount => stringCount;
        /// <summary>
        /// 额外信息 4字节
        /// </summary>
        public byte[] Unknow => unknow;
        /// <summary>
        /// 语言信息
        /// </summary>
        public int Language => language;

        //public List<Label> Labels => labels = new List<Label>();
        public Dictionary<Label, List<Value>> Labels => labels = new Dictionary<Label, List<Value>>();
        #endregion
        #region Method
        public virtual void AddLabel(Label label,List<Value> values)
        {
            Labels.Add(label, values);
        }
        public virtual void AddValue(Label label,Value value)
        {
            Labels.TryGetValue(label, out List<Value> values);
            values.Add(value);            
        }

        public virtual void LoadFile(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                // Header
                {
                    flag = Encoding.ASCII.GetString(ms.Read(4));
                    version = BitConverter.ToInt32(ms.Read(4), 0);
                    labelCount = BitConverter.ToInt32(ms.Read(4), 0);
                    stringCount = BitConverter.ToInt32(ms.Read(4), 0);
                    unknow = ms.Read(4);
                    language = BitConverter.ToInt32(ms.Read(4), 0);
                }
                // Body
                labels = new Dictionary<Label, List<Value>>();
                for (int i = 0; i < labelCount; i++)
                {
                    var lflag = Encoding.ASCII.GetString(ms.Read(4));
                    var lcount = BitConverter.ToInt32(ms.Read(4), 0);// Strings Count
                    var llenght = BitConverter.ToInt32(ms.Read(4), 0);// Label Name Lenght
                    var lname = Encoding.ASCII.GetString(ms.Read(llenght));

                    var lvalues = new List<Value>(lcount);

                    for (int j = 0; j < lcount; j++)
                    {
                        var sflag = Encoding.ASCII.GetString(ms.Read(4));
                        var slenght = BitConverter.ToInt32(ms.Read(4), 0);
                        var svalue = Encoding.Unicode.GetString(Decoding(slenght, ms.Read(slenght * 2)));
                        if (sflag[0] != 32)
                        {
                            var elenght = BitConverter.ToInt32(ms.Read(4), 0);
                            var evalue = Encoding.ASCII.GetString(ms.Read(elenght));
                            lvalues[j] = new Value(sflag, slenght, svalue, elenght, evalue);
                        }
                        else
                        {
                            lvalues[j] = new Value(sflag, slenght, svalue);
                        }
                    }
                    labels.Add(new Label(lflag, lcount, llenght, lname), lvalues);
                }
            }
        }
        public virtual async Task LoadFileAsync(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                {
                    var tflag = ms.ReadAsync(4);
                    var tver = ms.ReadAsync(4);
                    var tlcount = ms.ReadAsync(4);
                    var tscount = ms.ReadAsync(4);
                    var tunknow = ms.ReadAsync(4);
                    var tlang = ms.ReadAsync(4);

                    flag = Encoding.ASCII.GetString(await tflag);
                    version = BitConverter.ToInt32(await tver, 0);
                    labelCount = BitConverter.ToInt32(await tlcount, 0);
                    stringCount = BitConverter.ToInt32(await tscount, 0);
                    unknow = await tunknow;
                    language = BitConverter.ToInt32(await tlang, 0);
                }
                // ===
                labels = new Dictionary<Label, List<Value>>();
                for (int i = 0; i < labelCount; i++)
                {
                    var tlflag = ms.ReadAsync(4);
                    var tlcount = ms.ReadAsync(4);
                    var tllenght = ms.ReadAsync(4);

                    var lflag = Encoding.ASCII.GetString(await tlflag);
                    var lcount = BitConverter.ToInt32(await tlcount, 0);
                    var llenght = BitConverter.ToInt32(await tllenght, 0);
                    var lname = Encoding.ASCII.GetString(await ms.ReadAsync(llenght));

                    var lvalues = new List<Value>(lcount);
                    for (int j = 0; j < lcount; j++)
                    {
                        var sflag = Encoding.ASCII.GetString(await ms.ReadAsync(4));
                        var slenght = BitConverter.ToInt32(await ms.ReadAsync(4), 0);
                        var svalue = Encoding.Unicode.GetString(Decoding(slenght, await ms.ReadAsync(slenght * 2)));
                        if (sflag[0] != 32)
                        {
                            var elenght = BitConverter.ToInt32(await ms.ReadAsync(4), 0);
                            var evalue = Encoding.ASCII.GetString(await ms.ReadAsync(elenght));
                            lvalues[j] = new Value(sflag, slenght, svalue, elenght, evalue);
                        }
                        else
                        {
                            lvalues[j] = new Value(sflag, slenght, svalue);
                        }
                    }
                    labels.Add(new Label(lflag, lcount, llenght, lname), lvalues);
                }
            }
        }
        /// <summary>
        /// 编/解码
        /// </summary>
        /// <param name="ValueLength">长度</param>
        /// <param name="ValueData">内容</param>
        /// <returns>编/解码后的数组</returns>
        private static byte[] Decoding(int ValueLength, byte[] ValueData)
        {
            int ValueDataLength = ValueLength << 1;
            for (int i = 0; i < ValueDataLength; ++i)
            {
                ValueData[i] = (byte)~ValueData[i];
            }
            return ValueData;
        }
        #endregion
    }
    public static class Extension
    {
        public static byte[] Read(this MemoryStream ms, int count)
        {
            var bytes = new byte[count];
            ms.Read(bytes, 0, count);
            return bytes;
        }
        public static async Task<byte[]> ReadAsync(this MemoryStream ms, int count)
        {
            var bytes = new byte[count];
            await ms.ReadAsync(bytes, 0, count);
            return bytes;
        }
    }
}
