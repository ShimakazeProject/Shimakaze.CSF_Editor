using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shimakaze.ToolKit.CSF.Kernel;

namespace Shimakaze.ToolKit.CSF.GUI.ViewModel
{
    public class CsfDocument : INotifyPropertyChanged
    {
        private CsfClassFile classList;
        private CsfClassStruct labelList;
        private CsfLabelStruct value;
        private int classStringCount;

        public event PropertyChangedEventHandler PropertyChanged;
        public CsfDocument(CsfClassFile file)
        {
            ClassList = file;
        }
        public CsfClassFile ClassList
        {
            get => classList; set
            {
                classList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClassList)));
            }
        }
        public CsfClassStruct LabelList
        {
            get => labelList; set
            {
                labelList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelList)));
                Task.Run(RecountClassString);
            }
        }
        private void RecountClassString()
        {
            ClassStringCount = labelList.Select(i => i.Count).Sum();
        }
        public int ClassStringCount
        {
            get => classStringCount; private set
            {
                classStringCount = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClassStringCount)));
            }
        }
        public CsfLabelStruct Value
        {
            get => value; set
            {
                this.value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }
    }
}
