using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Shimakaze.ToolKit.CSF.GUI.Data
{
    public interface IEditStep
    {
        public string Name { get; }
        public string Summary { get; }
        public string Status { get; }
        public void Start();
    }
}
