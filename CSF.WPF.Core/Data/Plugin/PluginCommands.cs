using CSF.Logmgr;
using CSF.Plugin;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CSF.WPF.Core.Data.Plugin
{
    internal class PluginCommands
    {
        public static Command.RelayCommand<(Type, ConvertPlugin)> DocumentConvertCommand { get; } = new Command.RelayCommand<(Type, ConvertPlugin)>(async (plugin) =>
        {
            var type = plugin.Item1;
            try
            {
                Logger.Info("[Converter] Document\t:{0}", type);
                var returnValue = type.GetMethod(plugin.Item2.ExecuteName).Invoke(type, new object[] { PluginManager.Documents.SelectDocument.DocViewModel.TypeList });
                var typeSet = new Model.TypeSet();
                if (returnValue is Model.File file)
                {
                    typeSet.MakeType(file);
                }
                else if (returnValue is Task<Model.File> tFile)
                {
                    typeSet.MakeType(await tFile.ConfigureAwait(true));
                }
                else
                {
                    throw new TypeAccessException("Unknow Return Type.");
                }
                PluginManager.Documents.SelectDocument.DocViewModel.TypeList = typeSet;
                Logger.Info("[{0}]\tDONE.", nameof(Plugin));
            }
            catch (Exception ex)
            {
                Logger.Warn("[{0}]\t{1}\tPlugin Exception:{2}", nameof(Plugin), type, ex);
                MessageBox.Show(type.ToString(), "扩展程序错误:", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });

        public static Command.RelayCommand<(Type, ConvertPlugin)> LabelConvertCommand { get; } = new Command.RelayCommand<(Type, ConvertPlugin)>(async (plugin) =>
        {
            var type = plugin.Item1;
            try
            {
                Logger.Info("[Converter] LabelOnly\t:{0}", type);
                var source = PluginManager.Documents.SelectDocument.Document.DatasList.SelectedItems;
                for (int j = 0; j < source.Count; j++)
                {
                    var item = source[j] as Model.Label;
                    var newValues = new Model.Value[item.LabelValues.Length];
                    for (int i = 0; i < newValues.Length; i++)
                    {
                        Model.Value value = item.LabelValues[i];
                        object ret = type.GetMethod(plugin.Item2.SingleExecuteName).Invoke(type,
                            new object[] { value.ValueString });
                        if (ret is string valueStr)
                        {
                            newValues[i] = new Model.Value(valueStr, value.ExtraString);
                        }
                        else if (ret is Task<string> tValue)
                        {
                            newValues[i] = new Model.Value(await tValue.ConfigureAwait(true), value.ExtraString);
                        }
                    }
                    (PluginManager.Documents.SelectDocument.Document.DatasList.SelectedItems[j] as Model.Label)
                        .Changed(new Model.Label(item.LabelName, newValues));
                }
                PluginManager.Documents.SelectDocument.DocViewModel.Update();
            }
            catch (Exception ex)
            {
                Logger.Warn("Catched Plugin Exception \tMethod:{0} |Exception:{1}", type, ex);
                MessageBox.Show(type.ToString(), "扩展程序错误:", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });

        public static Command.RelayCommand<(Type, FilePlugin)> ImportCommand { get; } = new Command.RelayCommand<(Type, FilePlugin)>(async (plugin) =>
        {
            var type = plugin.Item1;
            FileDialog fileDialog;
            var converter = plugin.Item2;
            fileDialog = new OpenFileDialog();
            fileDialog.Filter = converter.Filter;

            Logger.Info("Importer Plugin is Running:\t{0}.", type);
            if (fileDialog.ShowDialog() ?? false)
            {
                try
                {
                    Logger.Info("Plugin : Selected File :\t{0}.", fileDialog.FileName);
                    object ret = type.GetMethod(converter.ExecuteName).Invoke(type, new object[] { fileDialog.FileName });
                    if (ret is Model.File)
                        PluginManager.Documents.Import(ret as Model.File);
                    else if (ret is Task<Model.File>)
                        PluginManager.Documents.Import(await (ret as Task<Model.File>).ConfigureAwait(true));
                    else
                    {
                        Logger.Warn("Plugin : Unknow Type.");
                        MessageBox.Show("操作未成功", "警告", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn("Catched Plugin Exception \tMethod:{0} |Exception:{1}", type, ex);
                    MessageBox.Show(type.ToString(), "扩展程序错误:", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                Logger.Info("Plugin : Cancel.");
                MessageBox.Show("操作取消", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        });

        public static Command.RelayCommand<(Type, FilePlugin)> ExportCommand { get; } = new Command.RelayCommand<(Type, FilePlugin)>(async (plugin) =>
        {
            var type = plugin.Item1;
            FileDialog fileDialog;
            var converter = plugin.Item2;
            fileDialog = new OpenFileDialog();
            fileDialog.Filter = converter.Filter;

            Logger.Info("[Exporter] Running:\t{0}.", type);
            if (fileDialog.ShowDialog() ?? false)
            {
                Logger.Info("Plugin : Selected File :\t{0}.", fileDialog.FileName);
                try
                {
                    object methodReturn = type.GetMethod(converter.ExecuteName).Invoke(type, new object[] { PluginManager.Documents.SelectDocument.DocViewModel.TypeList, fileDialog.FileName });
                    if (methodReturn is bool
                        ? (bool)methodReturn
                        : methodReturn is Task<bool>
                            ? await (methodReturn as Task<bool>).ConfigureAwait(true)
                            : false)
                    {
                        Logger.Info("Plugin : DONE.");
                        MessageBox.Show("操作成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        Logger.Info("Plugin : FAIL.");
                        MessageBox.Show("操作失败", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn("Catched Plugin Exception \tMethod:{0} |Exception:{1}", type, ex);
                    MessageBox.Show(type.ToString(), "扩展程序错误:", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                Logger.Info("Plugin : Cancel.");
                MessageBox.Show("操作取消", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        });


    }
}
