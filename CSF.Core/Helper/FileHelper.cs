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

        public static IFile CreateFile(byte[] header, byte[] body)
        {
            var iHeader = HeaderHelper.CreateHeader(header.ToArray());
            var labels = new ILabel[iHeader.LabelCount];
            int start = 0;
            for (int i = 0; i < iHeader.LabelCount; i++)
            {
                byte[] bytes;
                if ((bytes = body.Skip(start).ToArray()).Length > 0)
                {
                    var label = LabelHelper.CreateLabel(bytes);
                    int vl = 0;
                    foreach (var item in label.Values)
                        vl += (item.ValueLength * 2) + 0x08 + (item.ExtraLength ?? 0) + (item.ExtraLength != null ? 4 : 0);
                    start += 0x0C + label.NameLength + vl;
                    labels[i] = label;
                }
            }
            return new FileHelper(iHeader, labels);
        }
        public static IFile CreateFile(byte[] file) => CreateFile(file.Take(24).ToArray(), file.Skip(24).ToArray());
    }
}
