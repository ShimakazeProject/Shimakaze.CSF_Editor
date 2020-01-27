using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Core
{
    /// <summary>
    /// 文件头信息
    /// </summary>
    public interface IHeader
    {
        /// <summary>
        /// 文件标记 始终为" FSC"
        /// </summary>
        string Flag { get; }
        /// <summary>
        /// 文件版本
        /// </summary>
        int Version { get; }
        /// <summary>
        /// 标签数
        /// </summary>
        int LabelCount { get; }
        /// <summary>
        /// 字符串数
        /// </summary>
        int StringCount { get; }
        /// <summary>
        /// 额外信息 4字节
        /// </summary>
        byte[] Unknow { get; }
        /// <summary>
        /// 语言信息
        /// </summary>
        int Language { get; }
    }
}