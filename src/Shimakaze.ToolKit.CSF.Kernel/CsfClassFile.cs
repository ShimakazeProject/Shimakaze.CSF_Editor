using System;
using System.Collections.Generic;
using System.Text;

namespace Shimakaze.ToolKit.CSF.Kernel
{
    public sealed class CsfClassFile : CsfFileStruct, IList<CsfClassStruct>, ICollection<KeyValuePair<string, IReadOnlyList<CsfLabelStruct>>>
    {
        private List<CsfClassStruct> csfClass = new List<CsfClassStruct>();
        public IReadOnlyList<CsfClassStruct> Class => csfClass.AsReadOnly();

        public CsfClassStruct this[string key]
        {
            get
            {
                foreach (var item in csfClass) if (item.Name.Equals(key)) return item;
                return null;
            }

            set
            {
                for (int i = 0; i < csfClass.Count; i++)
                {
                    if (csfClass[i].Name.Equals(key))
                    {
                        csfClass[i] = value;
                        return;
                    }
                }
                csfClass.Add(value);
            }
        }
        CsfClassStruct IList<CsfClassStruct>.this[int index]
        {
            get => csfClass[index];
            set => csfClass[index] = value;
        }

        public override void Add(CsfLabelStruct label)
        {
            var type = label.Name.Split(':', '_')[0];
            this[type].Add(label);
            base.Add(label);
        }

        public void Add(CsfClassStruct item) => csfClass.Add(item);
        public void Add(KeyValuePair<string, IReadOnlyList<CsfLabelStruct>> item) => csfClass.Add(item);

        public bool Contains(CsfClassStruct item) => csfClass.Contains(item);
        public bool Contains(KeyValuePair<string, IReadOnlyList<CsfLabelStruct>> item) => csfClass.Contains(item);

        public bool ContainsKey(string key)
        {
            foreach (var item in csfClass)
                if (item.Name.Equals(key)) return true;
            return false;
        }

        public void CopyTo(CsfClassStruct[] array, int arrayIndex) => csfClass.CopyTo(array, arrayIndex);

        public int IndexOf(CsfClassStruct item) => csfClass.IndexOf(item);

        public void Insert(int index, CsfClassStruct item) => csfClass.Insert(index, item);
        public bool Remove(CsfClassStruct item) => csfClass.Remove(item);

        public bool Remove(string key)
        {
            foreach (var item in csfClass)
            {
                if (item.Name.Equals(key))
                {
                    Remove(item);
                    return true;
                }
            }
            return false;
        }
        public bool Remove(KeyValuePair<string, IReadOnlyList<CsfLabelStruct>> item) => csfClass.Remove(item);

        public bool TryGetValue(string key, out CsfClassStruct value)
        {
            foreach (var item in csfClass)
            {
                if (item.Name.Equals(key))
                {
                    value = item;
                    return true;
                }
            }
            value = null;
            return false;
        }

        IEnumerator<CsfClassStruct> IEnumerable<CsfClassStruct>.GetEnumerator() => csfClass.GetEnumerator();

        [Obsolete("不被使用")]
        public void CopyTo(KeyValuePair<string, IReadOnlyList<CsfLabelStruct>>[] array, int arrayIndex) => throw new NotImplementedException();
        [Obsolete("不被使用")]
        IEnumerator<KeyValuePair<string, IReadOnlyList<CsfLabelStruct>>> IEnumerable<KeyValuePair<string, IReadOnlyList<CsfLabelStruct>>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
