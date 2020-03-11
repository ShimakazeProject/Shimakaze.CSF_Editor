using MahApps.Metro.Controls.Dialogs;
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
using System.Windows.Shapes;

namespace CSF.WPF.Core.View
{
    /// <summary>
    /// EditDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditDialog
    {
        public EditDialog()
        {
            InitializeComponent();
        }
        public MainWindow Window;

        public static async Task<Model.Label> EditLabel(MainWindow Window, Model.Label label)
        {
            Logmgr.Logger.Info("[{0}]\tEdit", nameof(EditDialog));
            var dialog = new EditDialog();
            var dialogVM = dialog.DataContext as ViewModel.EditDialog;
            (dialog.DataContext as ViewModel.EditDialog).Close += () => dialog.CloseDialog(Window);
            (dialog.DataContext as ViewModel.EditDialog).SetLabel(label);
            await Window.ShowMetroDialogAsync(dialog);
            //await dialog.WaitForLoadAsync();
            //await dialog.WaitForCloseAsync();
            return await Task.Run(() =>
            {
                while (true)
                {
                    if (dialog.IsClose)
                    {
                        var result = dialogVM.GetLabel();
                        Logmgr.Logger.Info("[{0}]\tDone", nameof(EditDialog));
                        return result;
                    }
                }
            });
        }
        public bool IsClose { get; private set; } = false;
        public async void CloseDialog(MainWindow window)
        {
            IsClose = true;
            await this.RequestCloseAsync();
            await this.WaitForCloseAsync();
            await window.HideMetroDialogAsync(this);
        }
        private void Label_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(((TextBox)sender).Text, "^[^\\x21-\\x7E]*$"))
                TextChanged((TextBox)sender);
        }
        private void Extra_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(((TextBox)sender).Text, "^![^\\x20-\\x7E]*$"))
                TextChanged((TextBox)sender);
        }
        private static void TextChanged(TextBox sender)
        {
            if (string.IsNullOrEmpty(sender.Text)) return;
            sender.Text = sender.Text[0..^1];
            sender.CaretIndex = sender.Text.Length;
        }
    }
}
namespace CSF.WPF.Core.ViewModel
{
    using Data;
    using System.ComponentModel;
    public partial class EditDialog : INotifyPropertyChanged
    {
        private Model.Label Label;

        private ValueStruct selectedItem;
        private ValueStruct[] values;
        private string labelName;
        private bool canRemove;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action Close;
        public string LabelName
        {
            get => labelName; set
            {
                labelName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LabelName)));
            }
        }
        public ValueStruct[] Values
        {
            get => values; set
            {
                values = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Values)));
            }
        }
        public ValueStruct SelectedItem
        {
            get => selectedItem; set
            {
                selectedItem = value;
                CanRemove = (SelectedItem is null || values is null) ? false : values.Length > 1 ? true : false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
            }
        }

        public bool CanRemove
        {
            get => canRemove; set
            {
                canRemove = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanRemove)));
            }
        }

        public void SetLabel(Model.Label label)
        {
            Label = label;
            if (label is null) return;
            LabelName = label.LabelName;
            List<ValueStruct> values = new List<ValueStruct>(label.LabelValues.Length);
            foreach (var v in label.LabelValues)
            {
                values.Add(new ValueStruct { Extra = v.ExtraString, Value = v.ValueString, ID = values?.Count is null ? 0 : values.Count });
            }
            Values = values.ToArray();
            SelectedItem = Values[0];
        }
        public Model.Label GetLabel() => Label;
        public EditDialog()
        {
            OKCommand = new Command.RelayCommand(OK);
            RemoveCommand = new Command.RelayCommand(Remove);
            AddCommand = new Command.RelayCommand(Add);
            CancelCommand = new Command.RelayCommand(Cancel);
        }
        public Command.RelayCommand OKCommand { get; }
        public Command.RelayCommand CancelCommand { get; }
        public Command.RelayCommand RemoveCommand { get; }
        public Command.RelayCommand AddCommand { get; }
        private void OK()
        {
            var values = new Model.Value[this.values.Length];
            for (int i = 0; i < this.values.Length; i++)
            {
                values[i] = this.values[i];
            }
            Label = new Model.Label(labelName, values);
            Close?.Invoke();
        }
        public void Cancel()
        {
            Close?.Invoke();
            LabelName = null;
            Values = null;
        }

        public void Add()
        {
            var valueList = new List<ValueStruct>(values);
            var newValue = new ValueStruct { Value = "New Value", ID = values?.Length is null ? 0 : values.Length };
            valueList.Add(newValue);
            Values = valueList.ToArray();
            SelectedItem = newValue;
        }
        public void Remove()
        {
            var valueList = new List<ValueStruct>(values);
            valueList.Remove(selectedItem);
            Values = valueList.ToArray();
            SelectedItem = valueList[^1];
        }

    }
}
