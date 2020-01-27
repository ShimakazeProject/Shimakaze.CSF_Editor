using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Core.Helper
{
    public class FileHelper : IFile
    {
        public IHeader Header { get; private set; }

        public ILabel[] Labels { get; private set; }

        public FileHelper(IHeader header, IEnumerable<ILabel> labels)
        {
            Header = header;
            Labels = labels.ToArray();
        }

        public static IFile CreateFile(IEnumerable<byte> header, IEnumerable<byte> body)
        {
            var iHeader = HeaderHelper.CreateHeader(header.ToArray());
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
        public static IFile CreateFile(IEnumerable<byte> file) => CreateFile(file.Take(24), file.Skip(24));
    }
}
