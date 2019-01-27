using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheraEngine;
using TheraEngine.Core.Memory;

namespace System
{
    public static unsafe class StringExtension
    {
        public static bool EndsWithAny(this string s, string[] values, StringComparison comparisonType)
        {
            foreach (string value in values)
                if (s.EndsWith(value, comparisonType))
                    return true;
            return false;
        }
        /// <summary>
        /// Converts the elements of an array into a well-formatted list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="list">The array to format into a list.</param>
        /// <param name="separator">The separator to use to separate items in the list.</param>
        /// <param name="lastSeparator">The separator to use in the list between the last two elements.</param>
        /// <param name="elementToString">The method for converting individual array elements to strings.</param>
        /// <returns>A list of the elements in the array as a string.</returns>
        public static string ToStringList(this IList list, string separator, string lastSeparator, Func<object, string> elementToString)
        {
            //if (a.Length == 0)
            //    return string.Empty;

            //if (a.Length == 1)
            //    return elementToString(a[0]);

            //if (a.Length == 2)
            //    return elementToString(a[0]) + separator + elementToString(a[1]);

            return list.Cast<object>().Aggregate(new StringBuilder(),
                (builder, obj) => builder.Append(separator + elementToString(obj)),
                (builder) => builder.Remove(0, separator.Length).ToString());

            //string str = string.Empty;
            //string sep = separator;
            //for (int i = 0; i < list.Count; ++i)
            //{
            //    if (i == list.Count - 2)
            //        sep = lastSeparator;
            //    else if (i == list.Count - 1)
            //        sep = string.Empty;
            //    str += elementToString(list[i]) + sep;
            //}

            //return str;
        }
        /// <summary>
        /// Converts the elements of an array into a well-formatted list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="list">The array to format into a list.</param>
        /// <param name="separator">The separator to use to separate items in the list.</param>
        /// <param name="lastSeparator">The separator to use in the list between the last two elements.</param>
        /// <returns>A list of the elements in the array as a string.</returns>
        public static string ToStringList(this IList list, string separator, string lastSeparator)
        {
            if (list.Count == 0)
                return string.Empty;

            if (list.Count == 1)
                return list[0].ToString();

            if (list.Count == 2)
                return list[0].ToString() + separator + list[1].ToString();

            return list.Cast<object>().Aggregate(new StringBuilder(),
                (builder, obj) => builder.Append(separator + obj.ToString()),
                (builder) => builder.Remove(0, separator.Length).ToString());

            //string str = "";
            //string sep = separator;
            //for (int i = 0; i < a.Count; ++i)
            //{
            //    if (i == a.Count - 2)
            //        sep = lastSeparator;
            //    else if (i == a.Count - 1)
            //        sep = string.Empty;
            //    str += a[i].ToString() + sep;
            //}

            //return str;
        }
        /// <summary>
        /// Converts the elements of an array into a well-formatted list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="list">The array to format into a list.</param>
        /// <param name="separator">The separator to use to separate items in the list.</param>
        /// <param name="elementToString">The method for converting individual array elements to strings.</param>
        /// <returns>A list of the elements in the array as a string.</returns>
        public static string ToStringList(this IList list, string separator, Func<object, string> elementToString)
        {
            if (list.Count == 0)
                return string.Empty;

            if (list.Count == 1)
                return elementToString(list[0]);

            return list.Cast<object>().Aggregate(new StringBuilder(),
                (builder, obj) => builder.Append(separator + elementToString(obj)),
                (builder) => builder.Remove(0, separator.Length).ToString());

            //string str = "";
            //string sep = separator;
            //for (int i = 0; i < a.Count; ++i)
            //{
            //    if (i == a.Count - 1)
            //        sep = string.Empty;
            //    str += elementToString(a[i]) + sep;
            //}

            //return str;
        }
        /// <summary>
        /// Converts the elements of an array into a well-formatted list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="list">The array to format into a list.</param>
        /// <param name="separator">The separator to use to separate items in the list.</param>
        /// <returns>A list of the elements in the array as a string.</returns>
        public static string ToStringList(this IList list, string separator)
        {
            if (list.Count == 0)
                return string.Empty;

            if (list.Count == 1)
                return list[0].ToString();

            return list.Cast<object>().Aggregate(new StringBuilder(),
                (builder, obj) => builder.Append(separator + obj.ToString()),
                (builder) => builder.Remove(0, separator.Length).ToString());

            //string str = "";
            //string sep = separator;
            //for (int i = 0; i < list.Count; ++i)
            //{
            //    if (i == list.Count - 1)
            //        sep = string.Empty;
            //    str += list[i].ToString() + sep;
            //}

            //return str;
        }
        public static string GetExtensionLowercase(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            string ext = Path.GetExtension(path);
            if (ext.StartsWith("."))
                ext = ext.Substring(1);

            return ext.ToLowerInvariant();
        }
        public static bool IsAbsolutePath(this string path)
        {
            return !String.IsNullOrWhiteSpace(path)
                && path.IndexOfAny(Path.GetInvalidPathChars().ToArray()) == -1
                && Path.IsPathRooted(path)
                && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
        }
        public static bool StartsWithDirectorySeparator(this string str)
            => !string.IsNullOrEmpty(str) && str[0] == Path.DirectorySeparatorChar;
        public static bool EndsWithDirectorySeparator(this string str)
            => !string.IsNullOrEmpty(str) && str[str.Length - 1] == Path.DirectorySeparatorChar;
        public static string SplitCamelCase(this string str)
            => Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        public static bool IsValidExistingPath(this string path) => path.IsExistingDirectoryPath() != null;
        /// <summary>
        /// Determines the type of this path.
        /// <see langword="true"/> is a directory,
        /// <see langword="false"/> is a file,
        /// and <see langword="null"/> means the path is not valid.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool? IsExistingDirectoryPath(this string path)
        {
            //if (string.IsNullOrWhiteSpace(path))
            //    return null;
            //char[] invalid = { '<', '>', /*':',*/ '"', /*'\\', '/',*/ '|', '?', '*' };
            //if (path.IndexOfAny(invalid) >= 0)
            //    return null;
            //if (path.EndsWith(".") || path.EndsWith(" "))
            //    return null;
            if (Directory.Exists(path)) return true; //Is a folder 
            if (File.Exists(path)) return false; //Is a file
            return null; //Path is invalid
        }
        public static bool Equals(this string str, string other, bool ignoreCase)
        {
            return string.Equals(str, other, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);
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
            => str.Equals(other, StringComparison.OrdinalIgnoreCase);
        public static bool EqualsOrdinal(this string str, string other)
            => str.Equals(other, StringComparison.Ordinal);
        public static bool EqualsInvariantIgnoreCase(this string str, string other)
            => str.Equals(other, StringComparison.InvariantCultureIgnoreCase);
        public static bool EqualsInvariant(this string str, string other)
            => str.Equals(other, StringComparison.InvariantCulture);
        //public static bool IsNullOrEmpty(this string str)
        //{
        //    return string.IsNullOrEmpty(str);
        //}
        public static string MakeAbsolutePathRelativeTo(this string mainPath, string otherPath)
        {
            string[] mainParts = Path.GetFullPath(mainPath).Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            string[] otherParts = Path.GetFullPath(otherPath).Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            
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

            string newDir = string.Empty;
            for (int i = bias; i < otherParts.Length; ++i)
                newDir += Path.DirectorySeparatorChar + "..";
            for (int i = bias; i < mainLen; ++i)
                newDir += Path.DirectorySeparatorChar + mainParts[i];

            return newDir + Path.DirectorySeparatorChar.ToString() + fileName;
        }
        /// <summary>
        /// Parses the given string as an enum of the given type.
        /// </summary>
        public static T AsEnum<T>(this string s) where T : struct
            => (T)Enum.Parse(typeof(T), s);
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
            if (t.GetInterface(nameof(ISerializableString)) != null)
            {
                ISerializableString o = (ISerializableString)Activator.CreateInstance(t);
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
        public static VoidPtr Write(this string s, VoidPtr addr, bool nullTerminate)
        {
            sbyte* dPtr = (sbyte*)addr;
            foreach (char c in s)
                *dPtr++ = (sbyte)c;
            if (nullTerminate)
                *dPtr++ = 0;
            return dPtr;
        }
        public static void Write(this string s, ref VoidPtr addr, bool nullTerminate)
        {
            sbyte* dPtr = (sbyte*)addr;
            foreach (char c in s)
                *dPtr++ = (sbyte)c;
            *dPtr++ = 0;
            addr = dPtr;
        }
        public static void Write(this string s, ref sbyte* addr, bool nullTerminate)
        {
            foreach (char c in s)
                *addr++ = (sbyte)c;
            if (nullTerminate)
                *addr++ = 0;
        }
        public static void Write(this string s, ref sbyte* addr, int maxLength, bool nullTerminate)
        {
            for (int i = 0; i < Math.Max(s.Length, maxLength); ++i)
                *addr++ = (sbyte)s[i];
            if (nullTerminate)
                *addr++ = 0;
        }
        public static void Write(this string s, ref byte* addr, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(s);
            foreach (byte b in bytes)
                *addr++ = b;
        }
        public static int Write(this string s, byte* addr, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(s);
            foreach (byte b in bytes)
                *addr++ = b;
            return bytes.Length;
        }
        public static void Write(this string s, ref VoidPtr addr, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(s);
            foreach (byte b in bytes)
            {
                addr.Byte = b;
                addr += 1;
            }
        }
        public static int Write(this string s, VoidPtr addr, Encoding encoding)
        {
            byte[] bytes = encoding.GetBytes(s);
            foreach (byte b in bytes)
            {
                addr.Byte = b;
                addr += 1;
            }
            return bytes.Length;
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
        /// Finds the first instance that is the character passed.
        /// </summary>
        public static int FindFirst(this string str, int begin, Predicate<char> matchPredicate)
        {
            for (int i = begin; i < str.Length; ++i)
                if (matchPredicate(str[i]))
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
        /// Finds the first instance that is the string passed, searching forward in the string.
        /// </summary>
        public static int FindOccurrence(this string str, int begin, int occurrenceIndex, string searchStr)
        {
            int occurrence = 0;
            int offset = 0;
            for (int i = begin; i < str.Length; ++i)
            {
                bool found = true;
                offset = i;
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
                        return offset;
                    ++occurrence;
                }
            }
            return -1;
        }
        /// <summary>
        /// Finds all indices of the chars passed.
        /// </summary>
        public static int[] FindAllOccurrences(this string str, int firstIndex, int lastIndex, bool parallelSearch, params char[] searchChars)
        {
            if (parallelSearch)
            {
                ConcurrentBag<int> bag = new ConcurrentBag<int>();
                Parallel.For(firstIndex, lastIndex + 1, i =>
                {
                    if (searchChars.Any(x => x == str[i]))
                        bag.Add(i);
                });
                int[] array = bag.ToArray();
                Array.Sort(array);
                return array;
            }
            else
            {
                List<int> o = new List<int>();
                for (int i = firstIndex; i <= lastIndex; ++i)
                    if (searchChars.Any(x => x == str[i]))
                        o.Add(i);
                return o.ToArray();
            }
        }
        /// <summary>
        /// Finds the first instance that is the string passed, searching forward in the string.
        /// </summary>
        public static int[] FindAllOccurrences(this string str, int begin, string searchStr)
        {
            List<int> o = new List<int>();
            int firstIndex = 0;
            bool found;
            for (int i = begin; i < str.Length; ++i)
            {
                found = true;
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
                    o.Add(firstIndex);
            }
            return o.ToArray();
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
