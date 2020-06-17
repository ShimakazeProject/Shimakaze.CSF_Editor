using RA2CsfEditor.Command;
using RA2CsfEditor.Model;
using System;
using System.ComponentModel;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace RA2CsfEditor.ViewModel
{
    /// <summary>
    /// MainWindows 的 ViewModel
    /// </summary>
    public partial class MainVM
    {       
        #region 中继命令后半段
        private void Exit(Window win)
        {
            win.Close();
        }
        private async void Open()
        {
            IsIndeterminate = true;
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 开始打开");
            var ofd = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "RA2 字符串文件(*.csf)|*.csf"
            };
            var ofdret = ofd.ShowDialog();
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 尝试读取");
            if (ofdret ?? false)
                await ImportFromCsf(ofd.FileName);
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 结束");
            IsIndeterminate = false;
            Status = MaxStatus;
        }
        private async void Append()
        {
            IsIndeterminate = true;
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 开始打开");
            var ofd = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "RA2 字符串文件(*.csf)|*.csf"
            };
            var ofdret = ofd.ShowDialog();
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 尝试读取");
            if (ofdret ?? false)
                await AppendFromCsf(ofd.FileName);
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 结束");
            IsIndeterminate = false;
            Status = MaxStatus;
        }
        private async void Save()
        {
            IsIndeterminate = true;
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 保存");
            await ExportToCsf(_path);
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 结束");
            IsIndeterminate = false;
            Status = MaxStatus;
        }
        private async void SaveAs()
        {
            IsIndeterminate = true;
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 另存为");
            var sfd = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Json文件(*.csf)|*.csf",
                FileName = "Ra2md.csf"
            };
            var sfdret = sfd.ShowDialog();
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 关闭文件对话框");
            if (sfdret ?? false)
                await ExportToCsf(sfd.FileName);
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 结束");
            IsIndeterminate = false;
            Status = MaxStatus;
        }
        private void Close()
        {
            IsIndeterminate = true;
            ThisCSF = new CSF();
            Update();
            GC.Collect();
            IsIndeterminate = false;
        }
        private async void ExportJson()
        {
            IsIndeterminate = true;
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 导出到Json");
            var sfd = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Json文件(*.json)|*.json",
                FileName = "Ra2md.csf.json"
            };
            var sfdret = sfd.ShowDialog();
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 关闭文件对话框");
            if (sfdret ?? false)
            {
                await ExportToJson(sfd.FileName);
            }
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 结束");
            Status = MaxStatus;
        }
        private void ImportJson()
        {

            IsIndeterminate = true;
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 从Json导入");
            var ofd = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Json文件(*.json)|*.json"
            };
            var ofdret = ofd.ShowDialog();
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 关闭文件对话框");
            if (ofdret ?? false)
                ImportFromJson(ofd.FileName);
            IsIndeterminate = false;
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 结束");
            Status = MaxStatus;
        }
        private async void ExportXml()
        {
            IsIndeterminate = true;
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 导出到Xml");
            var sfd = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Xml 文件(*.xml)|*.xml",
                FileName = "Ra2md.csf.xml"
            };
            var sfdret = sfd.ShowDialog();
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 关闭文件对话框");
            if (sfdret ?? false)
                await ExportToXml(sfd.FileName);
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 结束");
            Status = MaxStatus;

        }
        private void ImportXml()
        {
            IsIndeterminate = true;
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 从Xml导入");
            var ofd = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Json文件(*.xml)|*.xml"
            };
            var ofdret = ofd.ShowDialog();
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 关闭文件对话框");
            if (ofdret ?? false)
            {
               //Task.Factory.StartNew(new Func<string,null>(
                ImportFromXml(ofd.FileName);
            }
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 结束");
            Status = MaxStatus;
        }
        private async void ExportTxt()
        {
            IsIndeterminate = true;
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 导出到Txt");
            var sfd = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "TXT 文件(*.txt)|*.txt",
                FileName = "Ra2md.txt"
            };
            var sfdret = sfd.ShowDialog();
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 关闭文件对话框");
            IsIndeterminate = false;
            if (sfdret ?? false)
            {
                await ThisCSF.SaveAsText(sfd.FileName);
                Update();
            }
            App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, "[MainVM] 结束");
            Status = MaxStatus;
        }
        private void EditLabel(Label label)
        {
            View.AddOrChangeWindow.ShowDialog((Label)label);
        }
        private void ReloadList()
        {
            IsIndeterminate = true;
            Update();
            GC.Collect();
            IsIndeterminate = false;
        }
        #endregion

        public MainVM()
        {
            Labels = new LabelStruct[0];
            SearchModeTitle = true;
            StringsNumber = "NULL";
            LabelsNumber = "NULL";
            //Update();
        }

        #region 用户方法
        public async Task AppendFromCsf(string path)
        {
            var addFile = new CSF();
            await addFile.LoadFromFile(path);
            await Task.WhenAll(
                from Label lbl
                in addFile.Label
                select new Task(() => ThisCSF.AddLabel(lbl)));
            Update();
        }

        private async void Search(string str) => await Task.Run(() =>
        {
            bool? mode = SearchModeTitle ? SearchModeValue ? (bool?)null : true : SearchModeValue ? false : (bool?)null;
            bool? regex = SearchModeRegex ? false : SearchModeFull ? true : (bool?)null;
            ThisCSF.Search(str, mode, regex);
        });
        #endregion
        #region 内部方法
        private async void Update()
        {
            Labels = new LabelStruct[0];
            SelectedLabel = null;
            await Task.WhenAll(
                Task.Run(() =>
                {
                    App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, $"[{nameof(Update)}({System.Threading.Thread.CurrentThread.ManagedThreadId})] 更新");
                    StringsNumber = ThisCSF?.Header.NumString.ToString() ?? "NULL";
                    LabelsNumber = ThisCSF?.Header.NumLabel.ToString() ?? "NULL";
                    App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, $"[{nameof(Update)}({System.Threading.Thread.CurrentThread.ManagedThreadId})] 更新Over");
                }),
                Task.Run(() =>
                {
                    App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, $"[{nameof(Update)}({System.Threading.Thread.CurrentThread.ManagedThreadId})] 更新列表");
                    MaxStatus = ThisCSF?.Header.NumLabel ?? 0;
                    if (MaxStatus != ThisCSF.Label.Length) return;
                    App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, $"[{nameof(Update)}({System.Threading.Thread.CurrentThread.ManagedThreadId})] 总数:{MaxStatus}");
                    IsIndeterminate = false;
                    for (Status = 0; Status < MaxStatus; Status++)
                    {
                        //Status = _nowNum / _count;//_count == 0 ? 0 :
                        var label = ThisCSF.Label[Status];
                        label.PropertyChanged += (o, e) =>
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Labels)));
                        Add(label);
                    }
                    IsIndeterminate = true;
                    SelectedLabel = Labels?.Length > 1 ? Labels[0] : null;
                    App.Logger.Write(Shimakaze.LogMgr.LogRank.TRACE, $"[{nameof(Update)}({System.Threading.Thread.CurrentThread.ManagedThreadId})] 更新完成");

                }));
            IsIndeterminate = false;
        }

        private void Add(Label label)
        {
            var tags = label.LabelString.Split(new char[] { ':', '_' });
            var tag = tags.Length != 1 ? tags[0] : "(default)";
            //if (Labels != null)
            for (int i = 0; i < Labels.Length; i++)
            {
                if (Labels[i].Type.ToUpper() == tag.ToUpper())
                {
                    Labels[i].Add(label);
                    return;
                }
            }
            var lbls = new LabelStruct(label);
            Labels = Labels.Concat(new LabelStruct[] { lbls }).ToArray();
            //if (lbls.Type == "(default)") SelectedItem = lbls;
        }
        #endregion


    }
}
