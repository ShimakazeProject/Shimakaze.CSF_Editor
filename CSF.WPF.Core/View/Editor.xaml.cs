using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;

namespace CSF.WPF.Core.View
{
    /// <summary>
    /// EditorDialog.xaml 的交互逻辑
    /// </summary>
    public partial class Editor
    {
        public Editor()
        {
            InitializeComponent();
            (DataContext as ViewModel.EditViewModel).PropertyChanged += Editor_PropertyChanged;
        }

        private Model.Label label;

        private void Editor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Label")
            {
                label = (DataContext as ViewModel.EditViewModel).Label;
            }
        }

        public void EditLabel(ViewModel.CsfDocViewModel docVM, Model.Label label)
        {
            (DataContext as ViewModel.EditViewModel).SetLabel(label);
        }

        public void AddLabel(ViewModel.CsfDocViewModel docVM)
        {
            var label = new Model.Label("New:Label", new Model.Value[] { "New Value" });
            (DataContext as ViewModel.EditViewModel).SetLabel(label);
            Task.Run(() =>
            {
            });
        }
    }
}
