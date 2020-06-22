using Shimakaze.ToolKit.CSF.Kernel.Extension;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace Shimakaze.ToolKit.CSF.Kernel
{
    /// <summary>
    /// CSF文件的标签结构
    /// </summary>
    public sealed class CsfLabelStruct : IList<CsfValueStruct>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        public const string CSF_LABEL_FLAG = " LBL";

        private readonly List<CsfValueStruct> values = new List<CsfValueStruct>();
        private string name = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

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
            PropertyChanged += (_, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(Values):
                        OnPropertyChanged(nameof(Count));
                        break;
                }
            };
        }
        public CsfLabelStruct(string name) : this() => Name = name;

        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index) => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item, index));
        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item) => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item));
        private void OnCollectionChanged(NotifyCollectionChangedAction action) => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
        /// <summary>
        /// 异步解析
        /// </summary>
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
        /// <summary>
        /// 异步还原
        /// </summary>
        public async Task DeparseAsync(Stream stream)
        {
            // 标签头
            await stream.WriteAsync(Encoding.ASCII.GetBytes(CSF_LABEL_FLAG));
            // 字符串数量 
            await stream.WriteAsync(BitConverter.GetBytes(Count));
            // 标签名长度
            await stream.WriteAsync(BitConverter.GetBytes(Name.Length));
            // 标签名
            await stream.WriteAsync(Encoding.ASCII.GetBytes(Name));

            foreach (var item in Values) await item.DeparseAsync(stream);
        }
        public CsfValueStruct this[int index]
        {
            get => values[index];
            set
            {
                values[index] = value;
                OnPropertyChanged(nameof(Values));
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, value, index);
            }
        }
        public int Count => values.Count;
        public void Add(CsfValueStruct item)
        {
            values.Add(item);
            item.PropertyChanged += (_, _) => OnPropertyChanged(nameof(Values));
            OnPropertyChanged(nameof(Values));
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
        }


        public void Clear()
        {
            values.Clear();
            OnPropertyChanged(nameof(Values));
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public bool Contains(CsfValueStruct item) => values.Contains(item);
        public void CopyTo(CsfValueStruct[] array, int arrayIndex) => values.CopyTo(array, arrayIndex);
        public int IndexOf(CsfValueStruct item) => IndexOf(item);
        public void Insert(int index, CsfValueStruct item)
        {
            values.Insert(index, item);
            OnPropertyChanged(nameof(Values));
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        public bool Remove(CsfValueStruct item)
        {
            var tmp = values.Remove(item);
            OnPropertyChanged(nameof(Values));
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
            return tmp;
        }

        public void RemoveAt(int index)
        {
            values.RemoveAt(index);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove);
            OnPropertyChanged(nameof(Values));
        }

        public IEnumerator<CsfValueStruct> GetEnumerator() => values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        [Obsolete("这个不被使用")]
        public bool IsReadOnly => false;
    }
}
