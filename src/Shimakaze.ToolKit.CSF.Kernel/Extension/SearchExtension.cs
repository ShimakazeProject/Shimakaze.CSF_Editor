using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

//using Shimakaze.ToolKit.CSF.Kernel.Extension;

namespace Shimakaze.ToolKit.CSF.Kernel.Extension
{
    public static class SearchExtension
    {
        /// <summary>
        /// 查找标签
        /// </summary>
        /// <param name="csf">文件</param>
        /// <param name="s">字符串</param>
        /// <param name="mode">搜索模式</param>
        /// <param name="similarity">模糊匹配相似度</param>
        public static void Search(this CsfFileStruct csf, string s, CsfSearchMode mode, double similarity = 0.5)
        {
            var label = (mode & CsfSearchMode.Label) != 0;
            var value = (mode & CsfSearchMode.Value) != 0;
            var extra = (mode & CsfSearchMode.Extra) != 0;
            var igoreCase = (mode & CsfSearchMode.IgoreCase) != 0;
            var keyword = (mode & CsfSearchMode.KeywordMatch) != 0;
            var fuzzy = (mode & CsfSearchMode.FuzzyMatch) != 0;
            var regex = (mode & CsfSearchMode.RegexMatch) != 0;
            var full = (mode & CsfSearchMode.FullMatch) != 0;

            var results = new List<(CsfLabelStruct, CsfSearchMode)>();
            (var comparison, var reg) = igoreCase
                ? (StringComparison.OrdinalIgnoreCase, new Regex(s, RegexOptions.IgnoreCase))
                : (StringComparison.Ordinal, new Regex(s));
            for (int i = 0; i < csf.Count; i++)
            {
                (var l, var result) = (csf[i], CsfSearchMode.None);
                if (label)
                {
                    if (full && l.Name.Equals(s, comparison))
                        result |= CsfSearchMode.Label | CsfSearchMode.FullMatch;
                    if (keyword && l.Name.Contains(s, comparison))
                        result |= CsfSearchMode.Label | CsfSearchMode.KeywordMatch;
                    if (fuzzy && l.Name.Compare(s) > similarity)
                        result |= CsfSearchMode.Label | CsfSearchMode.FuzzyMatch;
                    if (regex && reg.IsMatch(l.Name))
                        result |= CsfSearchMode.Label | CsfSearchMode.RegexMatch;
                }
                foreach (var v in l.Values)
                {
                    if (value)
                    {
                        if (full && v.Content.Equals(s, comparison))
                            result |= CsfSearchMode.Value | CsfSearchMode.FullMatch;
                        if (keyword && v.Content.Contains(s, comparison))
                            result |= CsfSearchMode.Value | CsfSearchMode.KeywordMatch;
                        if (fuzzy && v.Content.Compare(s) > similarity)
                            result |= CsfSearchMode.Value | CsfSearchMode.FuzzyMatch;
                        if (regex && reg.IsMatch(v.Extra))
                            result |= CsfSearchMode.Value | CsfSearchMode.RegexMatch;
                    }
                    if (extra)
                    {
                        if (full && v.Extra.Equals(s, comparison))
                            result |= CsfSearchMode.Extra | CsfSearchMode.FullMatch;
                        if (keyword && v.Extra.Contains(s, comparison))
                            result |= CsfSearchMode.Extra | CsfSearchMode.KeywordMatch;
                        if (fuzzy && v.Extra.Compare(s) > similarity)
                            result |= CsfSearchMode.Extra | CsfSearchMode.FuzzyMatch;
                        if (regex && reg.IsMatch(v.Extra))
                            result |= CsfSearchMode.Extra | CsfSearchMode.RegexMatch;
                    }
                }
                if (result != CsfSearchMode.None)
                {
                    if (igoreCase) result |= CsfSearchMode.IgoreCase;
                    results.Add((l, result));
                    continue;
                }
            }
        }
    }
}
