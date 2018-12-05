using System.Collections.Generic;
using System.Text;

namespace System
{
    public static unsafe class ArrayExtension
    {
        /// <summary>
        /// Converts the elements of an array into a well-formatted list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="a">The array to format into a list.</param>
        /// <param name="separator">The separator to use to separate items in the list.</param>
        /// <param name="lastSeparator">The separator to use in the list between the last two elements.</param>
        /// <param name="elementToString">The method for converting individual array elements to strings.</param>
        /// <returns>A list of the elements in the array as a string.</returns>
        public static string ToStringList<T>(this T[] a, string separator, string lastSeparator, Func<T, string> elementToString)
        {
            //if (a.Length == 0)
            //    return string.Empty;

            //if (a.Length == 1)
            //    return elementToString(a[0]);

            //if (a.Length == 2)
            //    return elementToString(a[0]) + separator + elementToString(a[1]);

            StringBuilder builder = new StringBuilder();
            string sep = separator;
            for (int i = 0; i < a.Length; ++i)
            {
                if (i == a.Length - 2)
                    sep = lastSeparator;
                else if (i == a.Length - 1)
                    sep = string.Empty;
                builder.Append(elementToString(a[i]) + sep);
            }

            return builder.ToString();
        }
        /// <summary>
        /// Converts the elements of an array into a well-formatted list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="a">The array to format into a list.</param>
        /// <param name="separator">The separator to use to separate items in the list.</param>
        /// <param name="lastSeparator">The separator to use in the list between the last two elements.</param>
        /// <returns>A list of the elements in the array as a string.</returns>
        public static string ToStringList<T>(this T[] a, string separator, string lastSeparator)
        {
            if (a.Length == 0)
                return string.Empty;

            if (a.Length == 1)
                return a[0].ToString();

            if (a.Length == 2)
                return a[0].ToString() + separator + a[1].ToString();

            StringBuilder builder = new StringBuilder();
            string sep = separator;
            for (int i = 0; i < a.Length; ++i)
            {
                if (i == a.Length - 2)
                    sep = lastSeparator;
                else if (i == a.Length - 1)
                    sep = string.Empty;
                builder.Append(a[i].ToString() + sep);
            }

            return builder.ToString();
        }
        /// <summary>
        /// Converts the elements of an array into a well-formatted list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="a">The array to format into a list.</param>
        /// <param name="separator">The separator to use to separate items in the list.</param>
        /// <param name="elementToString">The method for converting individual array elements to strings.</param>
        /// <returns>A list of the elements in the array as a string.</returns>
        public static string ToStringList<T>(this T[] a, string separator, Func<T, string> elementToString)
        {
            if (a.Length == 0)
                return string.Empty;

            if (a.Length == 1)
                return elementToString(a[0]);

            StringBuilder builder = new StringBuilder();
            string sep = separator;
            for (int i = 0; i < a.Length; ++i)
            {
                if (i == a.Length - 1)
                    sep = string.Empty;
                builder.Append(elementToString(a[i]) + sep);
            }

            return builder.ToString();
        }
        /// <summary>
        /// Converts the elements of an array into a well-formatted list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="a">The array to format into a list.</param>
        /// <param name="separator">The separator to use to separate items in the list.</param>
        /// <returns>A list of the elements in the array as a string.</returns>
        public static string ToStringList<T>(this T[] a, string separator)
        {
            if (a.Length == 0)
                return string.Empty;

            if (a.Length == 1)
                return a[0].ToString();
            
            return string.Join(separator, a);
        }
        /// <summary>
        /// Returns true if index >= 0 && index is less than length.
        /// Use this so you don't have to write that every time.
        /// </summary>
        public static bool IndexInArrayRange(this Array a, int value)
            => value >= 0 && value < a.Length;
        
        public static int[] FindAllOccurences(this Array a, object o)
        {
            List<int> l = new List<int>();
            int i = 0;
            foreach (object x in a)
            {
                if (x == o)
                    l.Add(i);
                i++;
            }
            return l.ToArray();
        }

        public static int[] FindAllMatchIndices<T>(this T[] a, Predicate<T> predicate)
        {
            List<int> list = new List<int>(a.Length);
            for (int i = 0; i < a.Length; ++i)
                if (predicate(a[i]))
                    list.Add(i);
            return list.ToArray();
        }

        public static int IndexOf(this Array a, object value)
            => Array.IndexOf(a, value);
        
        public static bool Contains(this string[] a, string value, StringComparison comp)
        {
            for (int i = 0; i < a.Length; ++i)
                if (string.Equals(a[i], value, comp))
                    return true;
            return false;
        }
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        public static T[] Append<T>(this T[] data, T[] appended)
        {
            T[] final = new T[data.Length + appended.Length];
            data.CopyTo(final, 0);
            appended.CopyTo(final, data.Length);
            return final;
        }
        public static T[] Resize<T>(this T[] data, int newSize)
        {
            Array.Resize(ref data, newSize);
            return data;
        }
        public static T[] FillWith<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = value;

            return array;
        }
        public static T[] FillWith<T>(this T[] array, Func<int, T> factory)
        {
            if (factory == null)
                return array;

            for (int i = 0; i < array.Length; i++)
                array[i] = factory(i);

            return array;
        }
        public static void ForEach<T>(this T[] data, Action<T> method)
        {
            if (data != null && method != null)
                Array.ForEach(data, method);
        }
    }
}
