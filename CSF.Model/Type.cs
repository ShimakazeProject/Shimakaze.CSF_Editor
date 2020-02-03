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
        private Dictionary<Label, List<Value>> labels;
        private string name;
        #endregion

        #region Construction
        public Type()
        {
            Visibility = true;
            Labels = new Dictionary<Label, List<Value>>();
        }
        public Type(Label label, List<Value> values)
        {
            Visibility = true;
            Labels = new Dictionary<Label, List<Value>>();
            var tag = label.LabelName.Split(new char[] { ':', '_' });
            Name = tag.Length != 1 ? tag[0].ToUpper() : "(default)";
            Add(label, values);
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
        public Dictionary<Label, List<Value>> Labels
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

        //#region Indexer
        //public Label this[int index]
        //{
        //    get => labels[index];
        //    set => labels[index] = value;
        //}
        //#endregion

        #region Method
        public virtual void Add(Label label,List<Value> values)
        {
            Labels.Add(label, values);
        }
        #endregion
    }
}
