using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CSF.Model.Extension.Xml
{
    internal class Import
    {
        //internal static Core.IFile V1(XmlElement xmlRoot)
        //{
        //    var labels =
        //    (from XmlNode label
        //    in xmlRoot.ChildNodes
        //    select new Label(
        //        (label as XmlElement).GetAttribute("Name"),
        //        (from XmlNode value in (label as XmlElement).ChildNodes
        //         select new Model.Value(
        //             (value as XmlElement).GetAttribute("String"),
        //             (value as XmlElement).GetAttribute("ExtraValue")
        //             )).ToArray())).ToArray();
        //    return new Core.Helper.FileHelper(
        //        Core.Helper.HeaderHelper.MakeHeader(Convert.ToInt32(xmlRoot.GetAttribute("Version")),
        //                                            labels.Length,
        //                                            (from label in labels select label.StringCount).Sum(),
        //                                            Convert.ToInt32(xmlRoot.GetAttribute("Language"))),
        //        labels);
        //}
    }
}
