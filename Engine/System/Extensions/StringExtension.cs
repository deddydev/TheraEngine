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
        /// <summary>
        /// Finds the first instance that is not the character passed.
        /// </summary>
        public static int FindFirstNot(this string str, int begin, char chr)
        {
            for (int i = begin; i < str.Length; ++i)
                if (str[i] != chr)
                    return i;
            return -1;
        }
        /// <summary>
        /// Finds the first instance that is not the character passed, searching backward in the string.
        /// </summary>
        public static int FindFirstNotReverse(this string str, int begin, char chr)
        {
            for (int i = begin; i >= 0; --i)
                if (str[i] != chr)
                    return i;
            return -1;
        }
        /// <summary>
        /// Finds the first instance that is the character passed.
        /// </summary>
        public static int FindFirst(this string str, int begin, char chr)
        {
            for (int i = begin; i < str.Length; ++i)
                if (str[i] == chr)
                    return i;
            return -1;
        }
        /// <summary>
        /// Finds the first instance that is the character passed, searching backward in the string.
        /// </summary>
        public static int FindFirstReverse(this string str, int begin, char chr)
        {
            for (int i = begin; i >= 0; --i)
                if (str[i] == chr)
                    return i;
            return -1;
        }
    }
}
