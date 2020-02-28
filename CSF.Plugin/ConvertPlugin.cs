using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.Plugin
{
    public abstract class ConvertPlugin : IPlugin
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public PluginType PluginType => PluginType.CONVERTER;
        public abstract string ExecuteName { get; }

        public abstract string SingleName { get; }
        public abstract string SingleDescription { get; }
        public abstract string SingleExecuteName { get; }
    }
}
