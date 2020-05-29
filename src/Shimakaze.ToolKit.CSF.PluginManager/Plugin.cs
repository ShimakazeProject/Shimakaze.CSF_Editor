using System;
using System.Collections.Generic;
using System.Text;

namespace Shimakaze.ToolKit.CSF.PluginManager
{
    public abstract class Plugin : IDisposable
    {
        protected bool disposedValue;

        public abstract string DllPath { get; set; }
        public abstract string PluginName { get; set; }
        public abstract string Author { get; set; }
        public abstract string Summary { get; set; }
        public abstract string Version { get; set; }

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
        // ~Plugin()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
