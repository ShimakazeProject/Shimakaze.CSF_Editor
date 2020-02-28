using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;


namespace CSF.Plugin
{
    public interface IPlugin //: IDisposable
    {
        /// <summary>
        /// 插件名
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 插件描述
        /// </summary>
        string Description { get; }
        /// <summary>
        /// 插件类型
        /// </summary>
        PluginType PluginType { get; }
        /// <summary>
        /// 执行方法名
        /// </summary>
        string ExecuteName { get; }
    }
    [Flags]
    public enum PluginType : byte
    {
        NONE = 0,
        EXPORTER = 1 << 0,
        IMPORTER = 1 << 1,
        CONVERTER = 1 << 2
    }
}
