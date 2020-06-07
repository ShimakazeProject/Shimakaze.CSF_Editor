using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shimakaze.ToolKit.CSF.Kernel
{
    public sealed class CsfClassStruct : IList<CsfLabelStruct>
    {
        /// <summary>
        /// 类型名
        /// </summary>
        public string Name { get; set; }
        private readonly List<CsfLabelStruct> labels = new List<CsfLabelStruct>();
        /// <summary>
        /// 类型的标签
        /// </summary>
        public IReadOnlyList<CsfLabelStruct> Labels => labels.AsReadOnly();
        public CsfClassStruct()
        {
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

        public int IndexOf(CsfLabelStruct item) => labels.IndexOf(item);
        public void Insert(int index, CsfLabelStruct item) => labels.Insert(index, item);
        public void RemoveAt(int index) => labels.RemoveAt(index);
        public void Add(CsfLabelStruct item) => labels.Add(item);
        public void Clear() => labels.Clear();
        public bool Contains(CsfLabelStruct item) => labels.Contains(item);
        public void CopyTo(CsfLabelStruct[] array, int arrayIndex) => labels.CopyTo(array, arrayIndex);
        public bool Remove(CsfLabelStruct item) => labels.Remove(item);
        public IEnumerator<CsfLabelStruct> GetEnumerator() => labels.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static implicit operator KeyValuePair<string, IReadOnlyList<CsfLabelStruct>>(CsfClassStruct csfClass) => new KeyValuePair<string, IReadOnlyList<CsfLabelStruct>>(csfClass.Name, csfClass.Labels);
        public static implicit operator CsfClassStruct(KeyValuePair<string, IReadOnlyList<CsfLabelStruct>> keyValuePair) => new CsfClassStruct(keyValuePair.Key, keyValuePair.Value);
    }
}
