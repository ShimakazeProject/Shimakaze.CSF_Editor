using System;
using System.Threading.Tasks;

namespace Shimakaze.ToolKit.CSF.Plugin.Interfaces
{
    public interface IPluginCommandInfo
    {
        /// <summary>
        /// 命令名
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 描述
        /// </summary>
        string Description { get; }

        void Execute();
    }
}
