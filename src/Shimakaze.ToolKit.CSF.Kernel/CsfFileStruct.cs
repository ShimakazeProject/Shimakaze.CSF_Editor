using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shimakaze.ToolKit.CSF.Kernel
{
    public abstract class CsfFileStruct : IList<CsfLabelStruct>, INotifyPropertyChanged
    {
        public CsfHeadStruct Head
        {
            get => head; set
            {
                head = value;
                OnPropertyChanged(nameof(Head));
            }
        }
        private readonly List<CsfLabelStruct> body = new List<CsfLabelStruct>();
        private CsfHeadStruct head = new CsfHeadStruct();

        public event PropertyChangedEventHandler PropertyChanged;
        public CsfFileStruct()
        {
            PropertyChanged += CsfFileStruct_PropertyChanged;
        }

        private void CsfFileStruct_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Body):
                    OnPropertyChanged(nameof(Count));
                    break;
            }
        }

        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public IReadOnlyList<CsfLabelStruct> Body => body.AsReadOnly();
        public CsfLabelStruct this[int index]
        {
            get => body[index];
            set => body[index] = value;
        }


        public async Task ParseAsync(Stream stream)
        {
            Head = await CsfHeadStruct.ParseAsync(stream);

            for (int i = 0; i < Head.LabelCount; i++)
                Add(await CsfLabelStruct.ParseAsync(stream));
        }

        public int Count => body.Count;
        [Obsolete("不被使用")]
        public bool IsReadOnly => throw new NotImplementedException();

        public virtual void Add(CsfLabelStruct item)
        {
            Head.LabelCount++;
            Head.StringCount += item.Count;
            body.Add(item);
            item.PropertyChanged += Body_PropertyChanged;
            OnPropertyChanged(nameof(Body));
        }

        private void Body_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Body));
        }

        public virtual void Clear()
        {
            Head.LabelCount = 0;
            Head.StringCount = 0;
            body.Clear();
            OnPropertyChanged(nameof(Body));
        }

        public virtual bool Contains(CsfLabelStruct item) => body.Contains(item);

        public virtual void CopyTo(CsfLabelStruct[] array, int arrayIndex) => body.CopyTo(array, arrayIndex);



        public virtual int IndexOf(CsfLabelStruct item) => body.IndexOf(item);
        public virtual void Insert(int index, CsfLabelStruct item)
        {
            body.Insert(index, item);
            Head.LabelCount++;
            Head.StringCount += item.Count;
            OnPropertyChanged(nameof(Body));
        }

        public virtual bool Remove(CsfLabelStruct item)
        {
            Head.LabelCount++;
            Head.StringCount += item.Count;
            var tmp = body.Remove(item);
            OnPropertyChanged(nameof(Body));
            return tmp;
        }

        public virtual void RemoveAt(int index)
        {
            var tmp = body[index];
            Head.LabelCount++;
            Head.StringCount += tmp.Count;
            body.RemoveAt(index);
            OnPropertyChanged(nameof(Body));
        }
        public virtual IEnumerator<CsfLabelStruct> GetEnumerator() => body.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
