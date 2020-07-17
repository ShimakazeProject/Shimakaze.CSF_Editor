using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.Win32;

using Shimakaze.ToolKit.CSF.GUI.ViewModel;
using Shimakaze.ToolKit.CSF.Kernel;

namespace Shimakaze.ToolKit.CSF.GUI.Data
{
    public static class DocumentManager
    {
        public static List<DocumentPage> Documents { get; } = new List<DocumentPage>();

    }
}
