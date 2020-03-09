using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.Model
{
    public class Header
    {
        public const string FlagStr = " FSC";
        #region Public Properties
        /// <summary>
        /// 文件标记 始终为" FSC"
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// 标签数
        /// </summary>
        public int LabelCount { get; set; }
        /// <summary>
        /// 语言信息
        /// </summary>
        public CsfLanguage Language { get; set; }
        /// <summary>
        /// 字符串数
        /// </summary>
        public int StringCount { get; set; }
        /// <summary>
        /// 额外信息 4字节
        /// </summary>
        public byte[] Unknow { get; set; }
        /// <summary>
        /// 文件版本
        /// </summary>
        public int Version { get; set; }
        #endregion Public Properties

        #region Public Methods
        public Header GetHeader() => this;
        #endregion Public Methods
    }
}
