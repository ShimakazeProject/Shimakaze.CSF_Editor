using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Shimakaze.ToolKit.CSF.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public static MainWindow LastInstance { get; private set; }
        public MainWindow()
        {
            LastInstance = this;
            InitializeComponent();
            _ = WaitRing();
        }
        private async Task WaitRing()
        {
            await Task.Delay(3000);
            _progressRing.Visibility = Visibility.Collapsed;
        }

    }
}