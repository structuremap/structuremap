using System;
using System.Reflection;
using System.Threading.Tasks;

namespace StructureMap.DynamicInterception
{
    internal static class ReflectionHelper
    {
        public static object CallPrivateStaticGenericMethod(this Type methodHolderType, string methodName,
            Type genericType, object parameter)
        {
            return methodHolderType
                .GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(genericType)
                .Invoke(null, new[] { parameter });
        }

        public static Type GetActualType(Type type)
        {
            if (IsNonGenericTask(type))
            {
                return typeof(void);
            }

            if (IsGenericTask(type))
            {
                return GetTypeFromGenericTask(type);
            }

            return type;
        }

        public static Type GetTypeFromGenericTask(Type type)
        {
            return type.GetGenericArguments()[0];
        }

        public static bool IsTask(Type type)
        {
            return typeof(Task).IsAssignableFrom(type);
        }

        public static bool IsNonGenericTask(Type type)
        {
            return type == typeof(Task);
        }

        public static bool IsGenericTask(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>);
        }
    }
}