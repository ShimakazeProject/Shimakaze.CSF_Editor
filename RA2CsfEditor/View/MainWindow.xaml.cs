using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace RA2CsfEditor.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowX
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.VM ?? new ViewModel.MainVM();
            //BindingOperations.SetBinding(pb_Status, ProgressBar.ValueProperty, new Binding("Status")
            //{
            //    Source = (DataContext as ViewModel.MainVM).LabelList,
            //    Mode= BindingMode.OneWay
            //});
            //Deactivated += (o, e) => Close();

        }
        private void InfoMenuItem_Click(object sender, RoutedEventArgs e)
        {
        }
        private void OutTXTMenuItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SearchMenuItem_Click(object sender, RoutedEventArgs e)
        {
        }


        private void AddMI_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ChangeMI_Click(object sender, RoutedEventArgs e)
        {
        }

        private void DropMI_Click(object sender, RoutedEventArgs e)
        {
        }
       
    }
}
