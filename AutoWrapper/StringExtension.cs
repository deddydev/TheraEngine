using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoWrapper
{
    public static class StringExtension
    {
        /// <summary>
        /// Returns the index of the first character that does not match c starting after index.
        /// </summary>
        public static int IndexOfNotAfter(this string s, char c, int index = 0)
        {
            for (int i = index + 1; i < s.Length; ++i)
                if (s[i] != c)
                    return i;
            return -1;
        }
        /// <summary>
        /// Returns the index of the last character that does not match c starting before index.
        /// </summary>
        public static int IndexOfNotBefore(this string s, char c, int index = 0)
        {
            for (int i = index - 1; i >= 0; --i)
                if (s[i] != c)
                    return i;
            return -1;
        }
        /// <summary>
        /// Returns the index of the last character that matches c starting before index.
        /// </summary>
        public static int IndexOfBefore(this string s, char c, int index)
        {
            for (int i = index - 1; i >= 0; --i)
                if (s[i] == c)
                    return i;
            return -1;
        }
        /// <summary>
        /// Returns the index of the first character that matches c starting after index.
        /// </summary>
        public static int IndexOfAfter(this string s, char c, int index)
        {
            for (int i = index + 1; i < s.Length; ++i)
                if (s[i] == c)
                    return i;
            return -1;
        }
        /// <summary>
        /// Gets the indices of all occurrences of the char in the string.
        /// </summary>
        public static List<int> OccurrencesOf(this string s, char c)
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < s.Length; ++i)
                if (s[i] == c)
                    indices.Add(i);
            return indices;
        }
    }
}
