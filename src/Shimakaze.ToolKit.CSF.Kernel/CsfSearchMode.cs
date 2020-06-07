using System;
using System.Collections.Generic;
using System.Text;

namespace Shimakaze.ToolKit.CSF.Kernel
{
    [Flags]
    public enum CsfSearchMode : uint
    {
        /// <summary>
        /// 无 - 什么也不会做
        /// </summary>
        None = 0,

        Label = 1,
        Value = 2,
        Extra = 4,

        IgoreCase = 32,

        KeywordMatch = 1024,
        FuzzyMatch = 2048,
        RegexMatch = 4096,
        FullMatch = 8192
    }
}
