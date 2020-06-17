using System;
using System.Collections.Generic;
using System.Text;

namespace Shimakaze.CSF.PluginManager
{
    class ManagedPlugin : Plugin
    {
        public override string DllPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string PluginName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Author { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Summary { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string Version { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
