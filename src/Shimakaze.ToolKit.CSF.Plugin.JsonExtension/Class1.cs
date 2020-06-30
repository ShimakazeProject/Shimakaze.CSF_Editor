using System;
using System.Collections.Generic;
using System.Diagnostics;

using Shimakaze.ToolKit.CSF.Plugin.Interfaces;

namespace Shimakaze.ToolKit.CSF.Plugin.JsonExtension
{
    public class PluginInfo : IPluginInfo
    {
        public string Name => "Json 扩展";

        public string Version => "3.0.0.0";

        public string Author => "舰队的偶像-岛风酱!";

        public string Description => "使CSF编辑器能够转换Json文件";

        protected List<IPluginCommandInfo> commands ;
        private bool disposedValue;

        public IList<IPluginCommandInfo> Commands => commands;

        public PluginInfo()
        {
            Trace.WriteLine("插件被构造");
        }
        public void Init()
        {
            Trace.WriteLine("插件被激活");
            commands = new List<IPluginCommandInfo>();
            commands.Add(new ImportCommand());
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~PluginInfo()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public class ImportCommand : IPluginCommandInfo
    {
        public string Name => "导入Json文件";

        public string Description => "从Json文件中反序列化一个CSF文件";

        public void Execute()
        {
            Trace.WriteLine("命令被激活");
        }
    }
}
