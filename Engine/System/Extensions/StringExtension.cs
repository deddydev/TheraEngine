using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace System
{
    public static unsafe class StringExtension
    {
        private static readonly Regex sWhitespace = new Regex(@"\s+");
        public static string ReplaceWhitespace(this string input, string replacement)
        {
            return sWhitespace.Replace(input, replacement);
        }
        public static void Write(this string s, ref sbyte* addr)
        {
            foreach (char c in s)
                *addr++ = (sbyte)c;
            *addr++ = 0;
        }
        public static void Write(this string s, ref sbyte* addr, int maxLength)
        {
            for (int i = 0; i < Math.Max(s.Length, maxLength); ++i)
                *addr++ = (sbyte)s[i];
            *addr++ = 0;
        }
    }
}
