using Shimakaze.ToolKit.CSF.Kernel.Extension;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace Shimakaze.ToolKit.CSF.Kernel
{
    /// <summary>
    /// CSF文件的标签结构
    /// </summary>
    public sealed class CsfLabelStruct : IList<CsfValueStruct>, INotifyPropertyChanged
    {
        public const string CSF_LABEL_FLAG = " LBL";

        private readonly List<CsfValueStruct> values = new List<CsfValueStruct>();
        private string name = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;

        public IReadOnlyList<CsfValueStruct> Values => values.AsReadOnly();
        public string Name
        {
            get => name; set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public CsfLabelStruct()
        {
            PropertyChanged += CsfLabelStruct_PropertyChanged;
        }
        public CsfLabelStruct(string name) : this()
        {
            Name = name;
        }

        private void CsfLabelStruct_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Values):
                    OnPropertyChanged(nameof(Count));
                    break;
            }
        }
        private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Values));
        }

        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
                lbl.Add(await CsfValueStruct.ParseAsync(stream));
            return lbl;
        }

        public CsfValueStruct this[int index]
        {
            get => values[index];
            set
            {
                values[index] = value;
                OnPropertyChanged(nameof(Values));
            }
        }
        public int Count => values.Count;
        public void Add(CsfValueStruct item)
        {
            values.Add(item);
            item.PropertyChanged += Value_PropertyChanged;
            OnPropertyChanged(nameof(Values));
        }


        public void Clear()
        {
            values.Clear();
            OnPropertyChanged(nameof(Values));
        }

        public bool Contains(CsfValueStruct item) => values.Contains(item);
        public void CopyTo(CsfValueStruct[] array, int arrayIndex) => values.CopyTo(array, arrayIndex);
        public int IndexOf(CsfValueStruct item) => IndexOf(item);
        public void Insert(int index, CsfValueStruct item)
        {
            values.Insert(index, item);
            OnPropertyChanged(nameof(Values));
        }

        public bool Remove(CsfValueStruct item)
        {
            var tmp = values.Remove(item);
            OnPropertyChanged(nameof(Values));
            return tmp;
        }

        public void RemoveAt(int index)
        {
            values.RemoveAt(index);
            OnPropertyChanged(nameof(Values));
        }

        public IEnumerator<CsfValueStruct> GetEnumerator() => values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        [Obsolete("这个不被使用")]
        public bool IsReadOnly => throw new NotImplementedException();
    }
}
