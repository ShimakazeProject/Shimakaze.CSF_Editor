using System;
using System.Collections.Generic;
using System.Text;
using Shimakaze.ToolKit.CSF.GUI.String;

namespace Shimakaze.ToolKit.CSF.GUI.Data
{
    public static class MainData
    {
        public static LocalString Local { get; private set; }

        public static void ChangeLocale(LocalString localString)
        {
            Local = localString;
        }
    }
}
