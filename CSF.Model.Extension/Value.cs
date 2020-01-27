using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSF.Model.Extension
{
    public static class Value
    {
        public static Model.Value[] ConvertArray(IEnumerable<string> strs) => (from str in strs select (Model.Value)str).ToArray();
        public static string[] ConvertArray(IEnumerable<Model.Value> strs) => (from str in strs select (string)str).ToArray();
    }
}
