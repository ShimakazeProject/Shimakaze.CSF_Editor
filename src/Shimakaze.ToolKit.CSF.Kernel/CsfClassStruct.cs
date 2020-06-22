using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Shimakaze.ToolKit.CSF.Kernel
{
    public sealed class CsfClassStruct : IList<CsfLabelStruct>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        /// <summary>
        /// 类型名
        /// </summary>
        public string Name
        {
            get => name; set
            {
                name = value.ToUpper();
                OnPropertyChanged(nameof(Name));
            }
        }
        private readonly List<CsfLabelStruct> labels = new List<CsfLabelStruct>();
        private string name;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index) => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item, index));
        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item) => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item));
        private void OnCollectionChanged(NotifyCollectionChangedAction action) => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// 类型的标签
        /// </summary>
        public IReadOnlyList<CsfLabelStruct> Labels => labels.AsReadOnly();
        public CsfClassStruct()
        {
            PropertyChanged += (_, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(Labels):
                        OnPropertyChanged(nameof(Count));
                        break;
                }
            };
        }

        public CsfClassStruct(string name) => Name = name;
        public CsfClassStruct(string name, IReadOnlyList<CsfLabelStruct> csfValues) : this(name) => labels = csfValues.ToList();
        public int Count => labels.Count;
        [Obsolete("这个不被使用")]
        public bool IsReadOnly => throw new NotImplementedException();

        public CsfLabelStruct this[int index]
        {
            get => labels[index];
            set
            {
                labels[index] = value;
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, value, index);
            }
        }


        public void Add(CsfLabelStruct item)
        {
            labels.Add(item);
            item.PropertyChanged += (_, _) => OnPropertyChanged(nameof(Labels));
            OnPropertyChanged(nameof(Labels));
            OnCollectionChanged(NotifyCollectionChangedAction.Replace, item);
        }

        private void Label_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        public void Clear()
        {
            labels.Clear();
            OnPropertyChanged(nameof(Labels));
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public bool Contains(CsfLabelStruct item) => labels.Contains(item);
        public void CopyTo(CsfLabelStruct[] array, int arrayIndex) => labels.CopyTo(array, arrayIndex);
        public int IndexOf(CsfLabelStruct item) => labels.IndexOf(item);
        public void Insert(int index, CsfLabelStruct item)
        {
            labels.Insert(index, item);
            OnPropertyChanged(nameof(Labels));
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        public void RemoveAt(int index)
        {
            labels.RemoveAt(index);
            OnPropertyChanged(nameof(Labels));
            OnCollectionChanged(NotifyCollectionChangedAction.Remove);
        }
        public bool Remove(CsfLabelStruct item)
        {
            var tmp = labels.Remove(item);
            OnPropertyChanged(nameof(Labels));
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
            return tmp;
        }

        public IEnumerator<CsfLabelStruct> GetEnumerator() => labels.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static implicit operator KeyValuePair<string, IReadOnlyList<CsfLabelStruct>>(CsfClassStruct csfClass) => new KeyValuePair<string, IReadOnlyList<CsfLabelStruct>>(csfClass.Name, csfClass.Labels);
        public static implicit operator CsfClassStruct(KeyValuePair<string, IReadOnlyList<CsfLabelStruct>> keyValuePair) => new CsfClassStruct(keyValuePair.Key, keyValuePair.Value);
    }
}
