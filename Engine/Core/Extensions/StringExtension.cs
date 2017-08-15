using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace System
{
    public static unsafe class StringExtension
    {
        public static bool? IsDirectory(this string path)
        {
            if (Directory.Exists(path)) return true; //Is a folder 
            else if (File.Exists(path)) return false; //Is a file
            else return null; //Path is invalid
        }
        public static decimal ParseInvariantDecimal(this string str)
        {
            return decimal.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
        }
        public static float ParseInvariantFloat(this string str)
        {
            return float.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
        }
        public static double ParseInvariantDouble(this string str)
        {
            return double.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
        }
        public static sbyte ParseInvariantSByte(this string str)
        {
            return sbyte.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
        }
        public static byte ParseInvariantByte(this string str)
        {
            return byte.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
        }
        public static short ParseInvariantShort(this string str)
        {
            return short.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
        }
        public static ushort ParseInvariantUShort(this string str)
        {
            return ushort.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
        }
        public static int ParseInvariantInt(this string str)
        {
            return int.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
        }
        public static uint ParseInvariantUInt(this string str)
        {
            return uint.Parse(str, CultureInfo.InvariantCulture.NumberFormat);
        }
        public static bool EqualsOrdinalIgnoreCase(this string str, string other)
        {
            return str.Equals(other, StringComparison.OrdinalIgnoreCase);
        }
        public static bool EqualsOrdinal(this string str, string other)
        {
            return str.Equals(other, StringComparison.Ordinal);
        }
        //public static bool IsNullOrEmpty(this string str)
        //{
        //    return string.IsNullOrEmpty(str);
        //}
        public static string MakePathRelativeTo(this string absolutePath, string relativePath)
        {
            string[] relParts = relativePath.Split('\\');
            string[] absParts = absolutePath.Split('\\');
            
            int absLen = absParts.Length;
            string fileName = absParts[absParts.Length - 1];
            if (fileName.Contains("."))
                --absLen;
            else
                fileName = "";

            int bias;
            for (bias = 0; bias < Math.Min(absLen, relParts.Length); ++bias)
                if (!absParts[bias].Equals(relParts[bias], StringComparison.InvariantCulture))
                    break;

            string newDir = "";
            for (int i = bias; i < relParts.Length; ++i)
                newDir += "..\\";
            for (int i = bias; i < absLen; ++i)
                newDir += absParts[i] + "\\";

            return newDir + fileName;
        }
        /// <summary>
        /// Parses the given string as an enum of the given type.
        /// </summary>
        public static T AsEnum<T>(this string s) where T : struct
        {
            return (T)Enum.Parse(typeof(T), s);
        }
        private static readonly Regex sWhitespace = new Regex(@"\s+");
        public static string ReplaceWhitespace(this string input, string replacement)
        {
            return sWhitespace.Replace(input, replacement);
        }
        public static VoidPtr Write(this string s, VoidPtr addr)
        {
            sbyte* dPtr = (sbyte*)addr;
            foreach (char c in s)
                *dPtr++ = (sbyte)c;
            *dPtr++ = 0;
            return dPtr;
        }
        public static void Write(this string s, ref VoidPtr addr)
        {
            sbyte* dPtr = (sbyte*)addr;
            foreach (char c in s)
                *dPtr++ = (sbyte)c;
            *dPtr++ = 0;
            addr = dPtr;
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
