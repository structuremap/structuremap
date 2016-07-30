using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.Graph;

namespace StructureMap.TypeRules
{

    public static partial class TypeExtensions
    {
        public static IEnumerable<Type> GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces;
        }

        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }

        public static IReadOnlyList<ConstructorInfo> GetConstructors(this Type type)
        {
            return new List<ConstructorInfo>(type.GetTypeInfo().DeclaredConstructors);
        }

        public static ConstructorInfo GetConstructor(this Type type, Type[] argumentTypes)
        {
            return type.GetTypeInfo().DeclaredConstructors
                .FirstOrDefault(ci => ci.ParametersMatch(argumentTypes));
        }

        public static MethodInfo GetMethod(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredMethod(name);
        }

        public static MethodInfo GetMethod(this Type type, string name, Type[] arguments)
        {
            return type.GetTypeInfo().DeclaredMethods.FirstOrDefault(mi => 
                mi.Name == name && 
                mi.ParametersMatch(arguments));
        }

        public static MethodInfo GetGetMethod(this PropertyInfo pi)
        {
            return pi.GetMethod;
        }

        public static MethodInfo GetSetMethod(this PropertyInfo pi)
        {
            return pi.SetMethod;
        }


        /* These need to be put back for CoreCLR
        public static IEnumerable<MethodInfo> GetMethods(this Type type)
        {
            return type.GetTypeInfo().DeclaredMethods;
        }

        public static PropertyInfo GetProperty(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredProperty(name);
        }

        public static IEnumerable<PropertyInfo> GetSettableProperties(this Type type)
        {
            return type.GetTypeInfo()
                .DeclaredProperties
                .Where(mi => mi.CanWrite && mi.SetMethod.IsPublic && !mi.SetMethod.IsStatic && mi.SetMethod.GetParameters().Length == 1);
        } 
        */

        public static IEnumerable<Type> GetExportedTypes(this Assembly assembly)
        {
            return assembly.ExportedTypes;
        }

        private static bool ParametersMatch(this MethodBase method, ICollection<Type> parameterTypes)
        {
            var origin = (ICollection<ParameterInfo>)method.GetParameters();
            return origin.Count == parameterTypes.Count &&
                   origin.Select(pi => pi.ParameterType).SequenceEqual(parameterTypes);
        }
    }

    /*
    public static class AssemblyLoader
    {
        public static Assembly ByName(string assemblyName)
        {
            return Assembly.Load(new AssemblyName(assemblyName));
        }
    }
    */
}
