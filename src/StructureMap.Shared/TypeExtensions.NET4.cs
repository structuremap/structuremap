using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.Graph;

namespace StructureMap.TypeRules
{
#if !NET45WP8
    public static partial class TypeExtensions
    {
        /// <summary>
        /// Many properties of the new TypeInfo class are equally named to the ones which were on type
        /// before. Adding this extension methods means that quite a few things can have equivalent syntax
        /// dependeing on whether you use TypeInfo or not.
        /// </summary>
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }

        public static IEnumerable<PropertyInfo> GetSettableProperties(this Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite && x.GetSetMethod(false) != null && x.GetSetMethod().GetParameters().Length == 1);
        } 
    }

    public static class AssemblyLoader
    {
        public static Assembly ByName(string assemblyName)
        {
            return Assembly.Load(assemblyName);
        }
    }
#endif
}
