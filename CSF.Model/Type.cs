using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CSF.Model
{
    public class Type : INotifyPropertyChanged, Interface.IVisibility
    {
        #region Field
        private bool visibility;
        private Label[] labels;
        private string name;
        #endregion

        #region Construction
        public Type()
        {
            Visibility = true;
            Labels = new Label[0];
        }
        public Type(Label label)
        {
            Visibility = true;
            Labels = new Label[0];
            var tag = label.LabelName.Split(new char[] { ':', '_' });
            Name = tag.Length != 1 ? tag[0].ToUpper() : "(default)";
            Add(label);
        }
        #endregion

        #region Property
        public bool Visibility
        {
            get => visibility; set
            {
                visibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Visibility)));
            }
        }
        public Label[] Labels
        {
            get => labels; set
            {
                labels = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Labels)));
            }
        }
        public string Name
        {
            get => name; private set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
        }
        #endregion

        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Indexer
        public Label this[int index]
        {
            get => labels[index];
            set => labels[index] = value;
        }
        #endregion

        #region Method
        public virtual void Add(Label label)
        {
            label.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(label.Visibility))
                {
                    Visibility = false;
                    foreach (var lbl in Labels)
                    {
                        if (lbl.Visibility)
                        {
                            Visibility = true;
                            break;
                        }
                    }
                }
            };
            Labels = Labels.Concat(new Label[] { label }).ToArray();
        }
        #endregion
    }
}
