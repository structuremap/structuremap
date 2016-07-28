using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StructureMap.TypeRules
{
    public static partial class TypeExtensions
    {
        public static IEnumerable<PropertyInfo> GetSettableProperties(this Type type)
        {
            return type.GetProperties()
                .Where(mi => mi.CanWrite && mi.SetMethod.IsPublic && !mi.SetMethod.IsStatic && mi.SetMethod.GetParameters().Length == 1);
        }
    }

    public static class AssemblyLoader
    {
        public static Assembly ByName(string assemblyName)
        {
            return Assembly.Load(new AssemblyName(assemblyName));
        }
    }
}
