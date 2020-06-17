using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA2CsfEditor.Model
{
    public class Header
    {
        public string Flag { get; private set; }
        public int Version { get; private set; }
        public int NumLabel { get; private set; }
        public int NumString { get; private set; }
        public byte[] Message { get; private set; }
        public int Language { get; private set; }
        /*
         * 0 =美国（英语）*
         * 1 =英国（英语）
         * 2 =德语*
         * 3 =法语*
         * 4 =西班牙语
         * 5 =义大利语
         * 6 =日语
         * 7 =贾伯沃基 
         * 8 =韩文*
         * 9 =中文*
         * > 9 =未知
         */
        /// <summary>
        /// 增加标签
        /// </summary>
        /// <param name="AddLBLNum">增加X个标签</param>
        public void AddLabel(int AddLBLNum)
        {
            NumLabel += AddLBLNum;
        }
        public void DropLabel(int DropLBLNum)
        {
            NumLabel -= DropLBLNum;
        }
        public void AddString(int AddLBLNum)
        {
            NumString += AddLBLNum;
        }
        public void DropString(int DropLBLNum)
        {
            NumString -= DropLBLNum;
        }
        public Header(string flag, int version, int numLable, int numString, byte[] message, int language)
        {
            Flag = flag;
            Version = version;
            NumLabel = numLable;
            NumString = numString;
            Message = message;
            Language = language;
        }
        public Header()
        {
        }
    }

}
