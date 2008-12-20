using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StructureMap
{
    public static class StringExtensions
    {
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
