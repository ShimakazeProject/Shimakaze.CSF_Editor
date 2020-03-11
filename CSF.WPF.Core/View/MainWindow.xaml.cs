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
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace CSF.WPF.Core.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            (DataContext as ViewModel.MainWindowViewModel).Documents.Window = this;
        }
        public async Task<Model.Label> EditLabel(Model.Label label)
        {
            return await EditDialog.EditLabel(this, label);
        }
    }
}
