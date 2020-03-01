using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CSF.WPF.Core.View
{
    /// <summary>
    /// CsfDoc.xaml 的交互逻辑
    /// </summary>
    public partial class CsfDoc 
    {
        public CsfDoc()
        {
            InitializeComponent();
            (DataContext as ViewModel.CsfDocViewModel).ThisView = this;
        }

        private void DatasList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var thisdg = sender as DataGrid;
            //(EditDialog.DataContext as ViewModel.EditViewModel).SetLabel(thisdg.SelectedItem as Model.Label);
            ((Window.GetWindow(this) as MainWindow).DataContext as ViewModel.MainWindowViewModel).EditLabel(DataContext as ViewModel.CsfDocViewModel, thisdg.SelectedItem as Model.Label);
        }
    }
}
