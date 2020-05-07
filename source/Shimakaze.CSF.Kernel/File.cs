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
        Head Head { get; set; }
        Label[] Body { get; set; }



        public static async Task<CSFFile> Parse(Stream stream)
        {
            var lbls = new List<Label>();
            var file = new CSFFile
            {
                Head = await Head.Parse(stream)
            };

            for (int i = 0; i < file.Head.LabelCount; i++)
                lbls.Add(await Label.Parse(stream));
            file.Body = lbls.ToArray();

            return file;
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
