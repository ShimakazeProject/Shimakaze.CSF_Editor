using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frg2089.CSF.Core
{
    public interface ILabel
    {
        ///// <summary>
        ///// 标签标记 始终为 " LBL"
        ///// </summary>
        //string LabelTag { get; }
        /// <summary>
        /// 字符串数
        /// </summary>
        int StringNum { get; }
        /// <summary>
        /// 标签名长度(ASCII)
        /// </summary>
        int LabelLength { get; }
        /// <summary>
        /// 标签名字符串
        /// </summary>
        string LabelName { get; set; }
        /// <summary>
        /// 字符串值标记 为" RTS"或"WRTS"
        /// </summary>
        string ValueTag { get; }
        /// <summary>
        /// 字符串值集合
        /// </summary>
        IEnumerable<IValue> Values { get; set; }
        /// <summary>
        /// 额外值长度(ASCII)
        /// </summary>
        int? ExtraValueLength { get; }
        /// <summary>
        /// 额外值字符串
        /// </summary>
        string ExtraValue { get; set; }
    }
}
