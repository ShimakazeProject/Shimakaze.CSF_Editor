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

namespace CSF.WPF.Framework.Dialog
{
    /// <summary>
    /// EditDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditDialog : Window
    {
        public EditDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Model.Value[] Values { get; set; }
        public string LabelName { get; set; }

        public static Model.Label ShowDialog(Model.Label lbl)
        {
            var editor = new EditDialog();
            editor.LabelName = lbl.LabelName;
            editor.Values = lbl.LabelValues;
            editor.ShowDialog();
            return new Model.Label(editor.LabelName, editor.Values);
        }
    }
}
