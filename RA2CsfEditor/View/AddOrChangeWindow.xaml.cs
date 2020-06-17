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
using System.Windows.Shapes;

namespace RA2CsfEditor.View
{
    /// <summary>
    /// AddOrChangeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddOrChangeWindow : WindowX
    {
        public AddOrChangeWindow()
        {
            InitializeComponent();
        }
        public AddOrChangeWindow(Model.Label label) : this()
        {
            this.DataContext = label;
        }

        public static Model.Label ShowDialog(Model.Label label)
        {
            var dialog = new AddOrChangeWindow(label);
            dialog.ShowDialog();
            return label;
        }
    }
}
