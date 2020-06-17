using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using Shimakaze.CSF.Kernel.Extension;

namespace Shimakaze.CSF.Kernel
{
    public class CSFFile
    {
        private Head _head;
        private List<Label> _body;
        public Head Head => _head;
        public Label[] Body => _body.ToArray();

        public void AddLabel(Label label)
        {
            _body.Add(label);
            _head.LabelCount++;
            _head.StringCount += label.StringCount;
        }
        public void DropLabel(Label label)
        {
            _body.Remove(label);
            _head.LabelCount--;
            _head.StringCount -= label.StringCount;
        }
        public void ChangeLabel(int index, Label label)
        {
            _body.RemoveAt(index);
            _body.Insert(index, label);
        }
        public void FindLabel(Label label)
        {
            _body.Find(obj => label.Equals(obj));
        }
        /// <summary>
        /// 按标签名查找
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="igoreCase">忽略大小写</param>
        /// <param name="fullMatch">全字匹配</param>
        /// <returns></returns>
        public Label[] SearchLabelName(string s, bool igoreCase = false, bool fullMatch = false)
        {
            return (igoreCase, fullMatch) switch
            {
                (true, true) => _body.FindAll(obj => obj.Name.Equals(s, StringComparison.OrdinalIgnoreCase)).ToArray(),
                (true, false) => _body.FindAll(obj => obj.Name.ToLower().Contains(s.ToLower())).ToArray(),
                (false, true) => _body.FindAll(obj => obj.Name.Equals(s)).ToArray(),
                (false, false) => _body.FindAll(obj => obj.Name.Contains(s)).ToArray()
            };
        }
        public Label[] SearchLabelName0(string s, bool igoreCase = false, bool fullMatch = false)
        {
            switch ((igoreCase, fullMatch))
            {
                case (true, true):
                    return _body.FindAll(obj => obj.Name.Equals(s, StringComparison.OrdinalIgnoreCase)).ToArray();
                case (true, false):
                    return _body.FindAll(obj => obj.Name.ToLower().Contains(s.ToLower())).ToArray();
                case (false, true):
                    return _body.FindAll(obj => obj.Name.Equals(s)).ToArray();
                case (false, false):
                    return _body.FindAll(obj => obj.Name.Contains(s)).ToArray();
            }
        }
        //ValueTuple<int,int>
        (int i, int j) Method(int i, int j)
        {
            var tmp = (Console.CursorLeft, Console.CursorTop);
            (Console.CursorLeft, Console.CursorTop) = tmp;
            (_, Console.CursorTop) = tmp;
            return (i, j);
        }

        public static async Task<CSFFile> Parse(Stream stream)
        {
            var lbls = new List<Label>();
            var file = new CSFFile
            {
                _head = await Head.Parse(stream)
            };

            for (int i = 0; i < file.Head.LabelCount; i++)
                lbls.Add(await Label.Parse(stream));
            file._body = lbls;

            return file;
        }
        public static CSFFile CreateFile()
        {
            var csf = new CSFFile();
            csf._head = Head.CreateHead(Head.Constants.CSF_VERSION_3, Head.Languages.en_US);
            csf._body = new List<Label>();
            return csf;
        }

        /// <summary>
        /// 值字符串 编/解码
        /// </summary>
        /// <param name="ValueLength">长度</param>
        /// <param name="ValueData">内容</param>
        /// <returns>编/解码后的数组</returns>
        public static byte[] Decoding(int ValueLength, byte[] ValueData)
        {
            int ValueDataLength = ValueLength << 1;
            for (int i = 0; i < ValueDataLength; ++i)
                ValueData[i] = (byte)~ValueData[i];
            return ValueData;
        }

    }
}
