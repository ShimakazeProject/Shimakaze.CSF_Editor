using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CSF.Model
{
    public sealed class Type : IEnumerable
    {
        public Type(string name, params Label[] labels)
        {
            Name = name;
            Labels = labels;
        }
        public string Name { get; private set; }
        public Label[] Labels { get; set; }


        public static Type operator +(Type left, Label right)
        {
            var labels = new List<Label>(left.Labels) { right };
            return new Type(left.Name, labels.ToArray());
        }


        #region Indexer
        public Label this[int index]
        {
            get => Labels[index];
            set => Labels[index] = value;
        }
        public Label this[string name]
        {
            get
            {
                for (int i = 0; i < Labels.Length; i++)
                {
                    if (Labels[i].LabelName.Equals(name))
                    {
                        return Labels[i];
                    }
                }
                return null;
            }
            set
            {
                for (int i = 0; i < Labels.Length; i++)
                {
                    if (Labels[i].LabelName.Equals(name))
                    {
                        Labels[i] = value;
                    }
                }
            }
        }
        #endregion
        public IEnumerator GetEnumerator() => Labels.GetEnumerator();
    }
}
