using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.Core
{
    public abstract class CsfFile
    {
        public abstract CsfHeader Header { get; }
        public abstract CsfLabel[] Labels { get; }

    }
}
