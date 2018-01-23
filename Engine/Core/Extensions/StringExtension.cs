using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using TheraEngine;

namespace System
{
    public static unsafe class StringExtension
    {
        public static bool EndsWithDirectorySeparator(this string str)
            => !string.IsNullOrEmpty(str) && str[str.Length - 1] == Path.DirectorySeparatorChar;
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
        public static string MakePathRelativeTo(this string mainPath, string otherPath)
        {
            string[] mainParts = Path.GetFullPath(mainPath).Split(Path.DirectorySeparatorChar);
            string[] otherParts = Path.GetFullPath(otherPath).Split(Path.DirectorySeparatorChar);
            
            int mainLen = mainParts.Length;
            string fileName = mainParts[mainParts.Length - 1];
            if (fileName.Contains("."))
                --mainLen;
            else
                fileName = "";

            //Find the first folder that does not match between the two paths
            int bias;
            for (bias = 0; bias < Math.Min(mainLen, otherParts.Length); ++bias)
                if (!mainParts[bias].Equals(otherParts[bias], StringComparison.InvariantCulture))
                    break;

            string newDir = Path.DirectorySeparatorChar.ToString();
            for (int i = bias; i < otherParts.Length; ++i)
                newDir += ".." + Path.DirectorySeparatorChar;
            for (int i = bias; i < mainLen; ++i)
                newDir += mainParts[i] + Path.DirectorySeparatorChar;

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
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (string.IsNullOrWhiteSpace(value))
                    return null;
                else
                    return value.ParseAs(t.GetGenericArguments()[0]);
            }
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
        /// Finds the first instance that is the string passed, searching backward in the string.
        /// </summary>
        public static int FindFirst(this string str, int begin, string searchStr)
        {
            int firstIndex = 0;
            for (int i = begin; i < str.Length; ++i)
            {
                bool found = true;
                firstIndex = i;
                for (int x = 0; x < searchStr.Length && i < str.Length; ++x, ++i)
                {
                    if (str[i] != searchStr[x])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                    return firstIndex;
            }
            return -1;
        }

        #region Find Occurrence
        /// <summary>
        /// Finds a specific instance that is the character passed.
        /// </summary>
        public static int FindOccurence(this string str, int begin, int occurrenceIndex, char chr)
        {
            int occurrence = 0;
            for (int i = begin; i < str.Length; ++i)
                if (str[i] == chr)
                {
                    if (occurrenceIndex == occurrence)
                        return i;
                    ++occurrence;
                }
            return -1;
        }
        /// <summary>
        /// Finds a specific instance that is the character passed.
        /// </summary>
        public static int FindOccurenceNot(this string str, int begin, int occurrenceIndex, char chr)
        {
            int occurrence = 0;
            for (int i = begin; i < str.Length; ++i)
                if (str[i] != chr)
                {
                    if (occurrenceIndex == occurrence)
                        return i;
                    ++occurrence;
                }
            return -1;
        }
        /// <summary>
        /// Finds the first instance that is the string passed, searching backward in the string.
        /// </summary>
        public static int FindOccurrence(this string str, int begin, int occurrenceIndex, string searchStr)
        {
            int occurrence = 0;
            int firstIndex = 0;
            for (int i = begin; i < str.Length; ++i)
            {
                bool found = true;
                firstIndex = i;
                for (int x = 0; x < searchStr.Length && i < str.Length; ++x, ++i)
                {
                    if (str[i] != searchStr[x])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    if (occurrenceIndex == occurrence)
                        return firstIndex;
                    ++occurrence;
                }
            }
            return -1;
        }
        /// <summary>
        /// Finds the first instance that is the string passed, searching backward in the string.
        /// </summary>
        public static int FindOccurrenceReverse(this string str, int begin, int occurrenceIndex, string searchStr)
        {
            int occurrence = 0;
            for (int i = begin; i >= 0; --i)
            {
                bool found = true;
                for (int x = searchStr.Length - 1; x >= 0 && i >= 0; --x, --i)
                {
                    if (str[i] != searchStr[x])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    if (occurrenceIndex == occurrence)
                        return i + 1;
                    ++occurrence;
                }
            }
            return -1;
        }
        #endregion

        #region Find First Reverse
        /// <summary>
        /// Finds the first instance that is the character passed, searching backward in the string.
        /// </summary>
        public static int FindFirstReverse(this string str, char chr)
        {
            return str.FindFirstReverse(str.Length - 1, chr);
        }
        /// <summary>
        /// Finds the first instance that is the string passed, searching backward in the string.
        /// </summary>
        public static int FindFirstReverse(this string str, string searchStr)
        {
            return str.FindFirstReverse(str.Length - 1, searchStr);
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
        /// <summary>
        /// Finds the first instance that is the string passed, searching backward in the string.
        /// </summary>
        public static int FindFirstReverse(this string str, int begin, string searchStr)
        {
            for (int i = begin; i >= 0; --i)
            {
                bool found = true;
                for (int x = searchStr.Length - 1; x >= 0 && i >= 0; --x, --i)
                {
                    if (str[i] != searchStr[x])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                    return i + 1;
            }
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
        #endregion

        /// <summary>
        /// Prints this string to the engine's output logs.
        /// </summary>
        /// <param name="str">The string to be printed.</param>
        /// <param name="args">Arguments for string.Format().</param>
        public static void Print(this string str, params object[] args)
            => Engine.Print(str, args);
        /// <summary>
        /// Prints this string to the engine's output logs and moves to the next line.
        /// </summary>
        /// <param name="str">The string to be printed.</param>
        /// <param name="args">Arguments for string.Format().</param>
        public static void PrintLine(this string str, params object[] args)
            => Engine.PrintLine(str, args);
    }
}
