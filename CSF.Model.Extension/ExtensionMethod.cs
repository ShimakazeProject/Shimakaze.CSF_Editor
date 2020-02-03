using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSF.Model.Extension
{
    public static class ExtensionMethod
    {
        //public static Model.Value[] ConvertArray(IEnumerable<string> strs) => (from str in strs select (Model.Value)str).ToArray();
        //public static string[] ConvertArray(IEnumerable<Model.Value> strs) => (from str in strs select (string)str).ToArray();


        public static string GetTypeName(this Label label)
        {
            var tag = label.LabelName.Split(new char[] { ':', '_' });
            return tag.Length != 1 ? tag[0].ToUpper() : "(default)";
        }
    }
}
