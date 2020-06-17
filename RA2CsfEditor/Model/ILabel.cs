using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RA2CsfEditor.Model
{
    public interface ILabel
    {
        string LabelTag { get; }
        int StringNum { get; }
        int LabelLength { get; }
        string LabelString { get; set; }
        string ValueTag { get; }
        Value[] Values { get; set; }
        int? ExtraValueLength { get; }
        string ExtraValue { get; set; }
        bool Visibility { get; set; }
    }
}
