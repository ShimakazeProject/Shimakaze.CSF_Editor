using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.Plugin
{
    public abstract class FilePlugin : IPlugin
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract PluginType PluginType { get; }
        public abstract string ExecuteName { get; }
        public abstract string Filter { get; }
    }
}
