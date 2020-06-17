using System;
using System.Collections.Generic;
using System.Text;

namespace Shimakaze.CSF.PluginManager
{
    class UnmanagedPlugin : Plugin
    {
        private int LibraryPath;
        public override string DllPath { get; set; }
        public override string PluginName { get; set; }
        public override string Author { get; set; }
        public override string Summary { get; set; }
        public override string Version { get; set; }
        public UnmanagedPlugin(string dllPath)
        {
            LibraryPath = DLLWrapper.LoadLibrary(DllPath = dllPath);
            PluginName = DLLWrapper.GetFunctionAddress<Func<string>>(LibraryPath, "GetPluginName")();
            Author = DLLWrapper.GetFunctionAddress<Func<string>>(LibraryPath, "GetPluginAuthor")();
            Summary = DLLWrapper.GetFunctionAddress<Func<string>>(LibraryPath, "GetPluginSummary")();
            Version = DLLWrapper.GetFunctionAddress<Func<string>>(LibraryPath, "GetPluginVersion")();
        }
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }
                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                DLLWrapper.FreeLibrary(LibraryPath);

                disposedValue = true;
            }
        }
    }
}
