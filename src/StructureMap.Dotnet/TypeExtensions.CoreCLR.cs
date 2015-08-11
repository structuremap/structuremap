using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StructureMap.TypeRules
{
    public static partial class TypeExtensions
    {
        public static IEnumerable<PropertyInfo> GetSettableProperties(this Type type)
        {
            return type.GetTypeInfo()
                .DeclaredProperties
                .Where(mi => mi.CanWrite && mi.SetMethod.IsPublic && !mi.SetMethod.IsStatic && mi.SetMethod.GetParameters().Length == 1);
        }
    }
}
