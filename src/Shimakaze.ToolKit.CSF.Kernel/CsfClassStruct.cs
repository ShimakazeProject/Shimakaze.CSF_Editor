using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Shimakaze.ToolKit.CSF.Kernel
{
    public sealed class CsfClassStruct : IList<CsfLabelStruct>, INotifyPropertyChanged
    {
        /// <summary>
        /// 类型名
        /// </summary>
        public string Name
        {
            get => name; set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private readonly List<CsfLabelStruct> labels = new List<CsfLabelStruct>();
        private string name;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 类型的标签
        /// </summary>
        public IReadOnlyList<CsfLabelStruct> Labels => labels.AsReadOnly();
        public CsfClassStruct()
        {
            PropertyChanged += CsfClassStruct_PropertyChanged;
        }

        private void CsfClassStruct_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Labels):
                    OnPropertyChanged(nameof(Count));
                    break;
            }
        }

        public CsfClassStruct(string name)
        {
            Name = name;
        }
        public CsfClassStruct(string name, IReadOnlyList<CsfLabelStruct> csfValues) : this(name)
        {
            labels = csfValues.ToList();
        }
        public int Count => labels.Count;
        [Obsolete("这个不被使用")]
        public bool IsReadOnly => throw new NotImplementedException();

        public CsfLabelStruct this[int index]
        {
            get => labels[index];
            set => labels[index] = value;
        }


        public void Add(CsfLabelStruct item)
        {
            labels.Add(item);
            item.PropertyChanged += Label_PropertyChanged;
            OnPropertyChanged(nameof(Labels));
        }

        private void Label_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Labels));
        }

        public void Clear()
        {
            labels.Clear();
            OnPropertyChanged(nameof(Labels));
        }

        public bool Contains(CsfLabelStruct item) => labels.Contains(item);
        public void CopyTo(CsfLabelStruct[] array, int arrayIndex) => labels.CopyTo(array, arrayIndex);
        public int IndexOf(CsfLabelStruct item) => labels.IndexOf(item);
        public void Insert(int index, CsfLabelStruct item)
        {
            labels.Insert(index, item);
            OnPropertyChanged(nameof(Labels));
        }

        public void RemoveAt(int index)
        {
            labels.RemoveAt(index);
            OnPropertyChanged(nameof(Labels));
        }
        public bool Remove(CsfLabelStruct item)
        {
            var tmp= labels.Remove(item);
            OnPropertyChanged(nameof(Labels));
            return tmp;
        }

        public IEnumerator<CsfLabelStruct> GetEnumerator() => labels.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static implicit operator KeyValuePair<string, IReadOnlyList<CsfLabelStruct>>(CsfClassStruct csfClass) => new KeyValuePair<string, IReadOnlyList<CsfLabelStruct>>(csfClass.Name, csfClass.Labels);
        public static implicit operator CsfClassStruct(KeyValuePair<string, IReadOnlyList<CsfLabelStruct>> keyValuePair) => new CsfClassStruct(keyValuePair.Key, keyValuePair.Value);
    }
}
