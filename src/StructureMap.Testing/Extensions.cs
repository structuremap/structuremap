using System;
using System.Collections.Generic;

namespace StructureMap.Testing
{
    // TODO -- eliminate in SM3
    internal static class StringExtensions
    {
        public static IEnumerable<T> Each<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T target in enumerable)
            {
                action(target);
            }

            return enumerable;
        }

        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNotEmpty(this string value)
        {
            return !value.IsEmpty();
        }

        public static string ToFormat(this string template, params object[] parameters)
        {
            return string.Format(template, parameters);
        }

        public static string[] ToDelimitedArray(this string content)
        {
            string[] array = content.Split(',');
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i].Trim();
            }

            return array;
        }
    }
}