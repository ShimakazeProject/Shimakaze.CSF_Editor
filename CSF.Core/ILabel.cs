using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Core
{
    public interface ILabel
    {
        /// <summary>
        /// 标签标记 始终为 " LBL"
        /// </summary>
        string LabelTag { get; }
        /// <summary>
        /// 字符串数
        /// </summary>
        int StringCount { get; }
        /// <summary>
        /// 标签名长度(ASCII)
        /// </summary>
        int NameLength { get; }
        /// <summary>
        /// 标签名字符串
        /// </summary>
        string LabelName { get; set; }
        /// <summary>
        /// 字符串值集合
        /// </summary>
        IValue[] Values { get; set; }
        /// <summary>
        /// 标签长度<para/>
        /// <code>int Length => 0x0C + NameLength + (from value in Values select value.Length).Sum();</code>
        /// </summary>
        int Length { get; }
    }
}
