using System;
using System.Collections.Generic;
using System.Text;

namespace CSF.Model
{
    public static class StringExtension
    {
        public static string Reverse(this string str)
        {
            StringBuilder reverse = new StringBuilder();
            for (int i = str.Length; i > 0; i--)
            {
                reverse.Append(str[i]);
            }
            return reverse.ToString();
        }
    }
}
