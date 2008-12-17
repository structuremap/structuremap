using System;
using System.Linq;

namespace StructureMap.DebuggerVisualizers
{
    public static class DisplayHelper
    {
        public static string AsCSharp(this Type type)
        {
            return type.AsCSharp(t => t.Name);
        }
        public static string AsCSharp(this Type type, Func<Type, string> selector)
        {
            var typeName = selector(type) ?? string.Empty;
            if (type.IsGenericType)
            {
                var genericParamSelector = type.IsGenericTypeDefinition ? t => t.Name : selector;
                var genericTypeList = String.Join(",", type.GetGenericArguments().Select(genericParamSelector).ToArray());
                var tickLocation = typeName.IndexOf('`');
                if (tickLocation >= 0)
                {
                    typeName = typeName.Substring(0, tickLocation);
                }
                return string.Format("{0}<{1}>", typeName, genericTypeList);
            }
            return typeName;
        }

    }
}
