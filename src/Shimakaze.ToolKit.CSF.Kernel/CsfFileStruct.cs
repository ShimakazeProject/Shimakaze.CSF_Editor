using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shimakaze.ToolKit.CSF.Kernel
{
    public abstract class CsfFileStruct : IList<CsfLabelStruct>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        public CsfHeadStruct Head
        {
            get => head; set
            {
                head = value;
                OnPropertyChanged(nameof(Head));
            }
        }
        protected readonly List<CsfLabelStruct> body = new List<CsfLabelStruct>();
        protected CsfHeadStruct head = new CsfHeadStruct();

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public CsfFileStruct()
        {
            PropertyChanged += CsfFileStruct_PropertyChanged;
        }

        protected void CsfFileStruct_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Body):
                    OnPropertyChanged(nameof(Count));
                    break;
            }
        }

        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        protected void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index) => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item, index));
        protected void OnCollectionChanged(NotifyCollectionChangedAction action, object item) => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item));
        protected void OnCollectionChanged(NotifyCollectionChangedAction action) => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
        public IReadOnlyList<CsfLabelStruct> Body => body.AsReadOnly();
        public virtual CsfLabelStruct this[int index]
        {
            get => body[index];
            set
            {
                body[index] = value;
                OnCollectionChanged(NotifyCollectionChangedAction.Replace, value, index);
            }
        }


        public virtual async Task ParseAsync(Stream stream)
        {
            Head = await CsfHeadStruct.ParseAsync(stream);

            for (int i = 0; i < Head.LabelCount; i++)
                AddNoChangeHead(await CsfLabelStruct.ParseAsync(stream));
        }

        public virtual async Task DeparseAsync(Stream stream)
        {
            await Head.DeparseAsync(stream);
            foreach (var item in Body) await item.DeparseAsync(stream);
        }
        public virtual int Count => body.Count;
        [Obsolete("不被使用")]
        public bool IsReadOnly => throw new NotImplementedException();
        public virtual void AddNoChangeHead(CsfLabelStruct item)
        {
            body.Add(item);
            item.PropertyChanged += (_, _) => OnPropertyChanged(nameof(Body));
            OnPropertyChanged(nameof(Body));
        }
        public virtual void Add(CsfLabelStruct item)
        {
            Head.LabelCount++;
            Head.StringCount += item.Count;
            AddNoChangeHead(item);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
        }

        public virtual void Clear()
        {
            Head.LabelCount = 0;
            Head.StringCount = 0;
            body.Clear();
            OnPropertyChanged(nameof(Body));
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
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
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        public virtual bool Remove(CsfLabelStruct item)
        {
            Head.LabelCount++;
            Head.StringCount += item.Count;
            var tmp = body.Remove(item);
            OnPropertyChanged(nameof(Body));
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
            return tmp;
        }

        public virtual void RemoveAt(int index)
        {
            var tmp = body[index];
            Head.LabelCount++;
            Head.StringCount += tmp.Count;
            body.RemoveAt(index);
            OnPropertyChanged(nameof(Body));
            OnCollectionChanged(NotifyCollectionChangedAction.Remove);
        }
        public virtual IEnumerator<CsfLabelStruct> GetEnumerator() => body.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
