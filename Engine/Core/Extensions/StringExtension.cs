using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using TheraEngine;

namespace System
{
    public static unsafe class StringExtension
    {
        public static string SplitCamelCase(this string str)
            => Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
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
        public static bool EqualsInvariantIgnoreCase(this string str, string other)
        {
            return str.Equals(other, StringComparison.InvariantCultureIgnoreCase);
        }
        public static bool EqualsInvariant(this string str, string other)
        {
            return str.Equals(other, StringComparison.InvariantCulture);
        }
        //public static bool IsNullOrEmpty(this string str)
        //{
        //    return string.IsNullOrEmpty(str);
        //}
        public static string MakePathRelativeTo(this string relativePath, string absolutePath)
        {
            string[] relParts = relativePath.Split(Path.DirectorySeparatorChar);
            string[] absParts = absolutePath.Split(Path.DirectorySeparatorChar);
            
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
                newDir += ".." + Path.DirectorySeparatorChar;
            for (int i = bias; i < absLen; ++i)
                newDir += absParts[i] + Path.DirectorySeparatorChar;

            return newDir + fileName;
        }
        /// <summary>
        /// Parses the given string as an enum of the given type.
        /// </summary>
        public static T AsEnum<T>(this string s) where T : struct
        {
            return (T)Enum.Parse(typeof(T), s);
        }
        public static T ParseAs<T>(this string value)
            => (T)value.ParseAs(typeof(T));
        public static object ParseAs(this string value, Type t)
        {
            if (t.GetInterface("IParsable") != null)
            {
                IParsable o = (IParsable)Activator.CreateInstance(t);
                o.ReadFromString(value);
                return o;
            }
            if (string.Equals(t.BaseType.Name, "Enum", StringComparison.InvariantCulture))
                return Enum.Parse(t, value);
            switch (t.Name)
            {
                case "Boolean": return Boolean.Parse(value);
                case "SByte": return SByte.Parse(value);
                case "Byte": return Byte.Parse(value);
                case "Char": return Char.Parse(value);
                case "Int16": return Int16.Parse(value);
                case "UInt16": return UInt16.Parse(value);
                case "Int32": return Int32.Parse(value);
                case "UInt32": return UInt32.Parse(value);
                case "Int64": return Int64.Parse(value);
                case "UInt64": return UInt64.Parse(value);
                case "Single": return Single.Parse(value);
                case "Double": return Double.Parse(value);
                case "Decimal": return Decimal.Parse(value);
                case "String": return value;
            }
            throw new InvalidOperationException(t.ToString() + " is not parsable");
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
        public static void Print(this string str, params object[] args)
        {
            Engine.Print(str, args);
        }
        public static void PrintLine(this string str, params object[] args)
        {
            Engine.PrintLine(str, args);
        }
    }
}
