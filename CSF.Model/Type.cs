using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CSF.Model
{
    public sealed class Type : IEnumerable, INotifyPropertyChanged
    {
        private Label[] labels;

        public Type(string name, params Label[] labels)
        {
            Name = name;
            Labels = labels;
        }
        public string Name { get; private set; }
        public Label[] Labels
        {
            get => labels; set
            {
                labels = value;
                foreach(var label in value)
                {
                    label.PropertyChanged += Label_PropertyChanged;
                }
            }
        }

        private void Label_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Visibility))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visibility)));
            }
        }

        public bool Visibility
        {
            get
            {
                foreach (var label in Labels)
                {
                    if (label.Visibility == true)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public static Type operator +(Type left, Label right)
        {
            var labels = new List<Label>(left.Labels) { right };
            return new Type(left.Name, labels.ToArray());
        }


        #region Indexer
        public Label this[int index]
        {
            get => Labels[index];
            set
            {
                Labels[index] = value;
                value.PropertyChanged += Label_PropertyChanged;
            }
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
                        value.PropertyChanged += Label_PropertyChanged;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        public IEnumerator GetEnumerator() => Labels.GetEnumerator();
    }
}
