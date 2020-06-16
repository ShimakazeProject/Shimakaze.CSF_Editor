using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;

namespace CSF.WPF.Core.Resources
{
    internal static class Resource
    {
        private static string NS = "CSF.WPF.Core.Resources.{0}.{1}.Resource.resx";
        private static Lang Language;
        public static void SetLanguage(Lang lang)
        {
            Language = lang;
        }

        public static string GetString(string key)
        {
            var resManager = new ResourceManager(string.Format(NS, "Strings", Language), typeof(Resource).Assembly);
            //可以不需要CultureInfo ，也可以获取到资源的值
            return resManager.GetString(key);
        }
        public enum Lang
        {
            Zh_Hans
        }
    }
}
