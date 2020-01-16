using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frg2089.CSF.Core.Helper
{
    public class FileHelper : IFile
    {
        public IHeader Header { get; private set; }

        public IEnumerable<ILabel> Labels { get; private set; }

        private FileHelper(IHeader header, IEnumerable<ILabel> labels)
        {
            Header = header;
            Labels = labels;
        }

        public static IFile CreateFile(IEnumerable<byte> header, IEnumerable<byte> body)
        {
            var iHeader = HeaderHelper.CreateHeader(header);
            var labels = new ILabel[iHeader.LabelCount];
            int start = 0;
            for (int i = 0; i < iHeader.LabelCount; i++)
            {
                var label = LabelHelper.CreateLabel(body.Skip(start));
                start += label.Length;
                labels[i] = label;
            }
            return new FileHelper(iHeader, labels);
        }
    }
}
