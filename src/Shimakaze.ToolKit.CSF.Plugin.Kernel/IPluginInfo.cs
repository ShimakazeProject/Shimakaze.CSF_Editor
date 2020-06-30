using System;
using System.Collections.Generic;

namespace Shimakaze.ToolKit.CSF.Plugin.Interfaces
{
    public interface IPluginInfo : IDisposable
    {
        /// <summary>
        /// 插件名
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 版本号
        /// </summary>
        string Version { get; }
        /// <summary>
        /// 作者
        /// </summary>
        string Author { get; }
        /// <summary>
        /// 描述
        /// </summary>
        string Description { get; }
        /// <summary>
        /// 命令
        /// </summary>
        IList<IPluginCommandInfo> Commands { get; }

        /// <summary>
        /// 初始化语句
        /// </summary>
        void Init();

    }
}
