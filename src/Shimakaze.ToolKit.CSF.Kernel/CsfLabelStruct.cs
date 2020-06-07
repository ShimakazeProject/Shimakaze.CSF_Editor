using Shimakaze.ToolKit.CSF.Kernel.Extension;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace Shimakaze.ToolKit.CSF.Kernel
{
    /// <summary>
    /// CSF文件的标签结构
    /// </summary>
    public sealed class CsfLabelStruct : IList<CsfValueStruct>
    {
        public const string CSF_LABEL_FLAG = " LBL";

        public string Name { get; set; } = string.Empty;
        private readonly List<CsfValueStruct> values= new List<CsfValueStruct>();
        public IReadOnlyList<CsfValueStruct> Values => values.AsReadOnly();


        public static async Task<CsfLabelStruct> ParseAsync(Stream stream)
        {
            var lbl = new CsfLabelStruct();
            string flag, name;
            int count, nameLength;
            // 标签头
            flag = Encoding.ASCII.GetString(await stream.ReadAsync(4));
            // 标签头对不上
            if (!flag.Equals(CSF_LABEL_FLAG)) throw new FormatException("Unknown File Format");
            // 字符串数量 
            count = BitConverter.ToInt32(await stream.ReadAsync(4), 0);
            // 标签名长度
            nameLength = BitConverter.ToInt32(await stream.ReadAsync(4), 0);
            // 标签名
            lbl.Name = name = Encoding.ASCII.GetString(await stream.ReadAsync(nameLength));
            for (int i = 0; i < count; i++)
                lbl.Add(await CsfValueStruct.Parse(stream));
            return lbl;
        }

        public CsfValueStruct this[int index]
        {
            get => values[index];
            set => values[index] = value;
        }
        public int Count => values.Count;
        [Obsolete("这个不被使用")]
        public bool IsReadOnly => throw new NotImplementedException();
        public void Add(CsfValueStruct item) => values.Add(item);
        void ICollection<CsfValueStruct>.Clear() => values.Clear();
        public bool Contains(CsfValueStruct item) => values.Contains(item);
        public void CopyTo(CsfValueStruct[] array, int arrayIndex) => values.CopyTo(array, arrayIndex);
        public int IndexOf(CsfValueStruct item) => IndexOf(item);
        public void Insert(int index, CsfValueStruct item) => values.Insert(index, item);
        public bool Remove(CsfValueStruct item) => values.Remove(item);
        public void RemoveAt(int index) => values.RemoveAt(index);
        public IEnumerator<CsfValueStruct> GetEnumerator() => values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
