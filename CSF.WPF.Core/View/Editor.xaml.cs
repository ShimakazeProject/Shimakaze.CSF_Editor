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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((Window.GetWindow(this) as MainWindow).DataContext as ViewModel.MainWindowViewModel).ShowEditor = false;
        }
    }
}
