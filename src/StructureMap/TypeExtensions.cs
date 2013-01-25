using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{


    internal static class BasicExtensions
    {
        public static string ToName(this ILifecycle lifecycle)
        {
            return lifecycle == null ? InstanceScope.Transient.ToString() : lifecycle.Scope;
        }


        public static void Fill<T>(this IList<T> list, T value)
        {
            if (list.Contains(value)) return;
            list.Add(value);
        }

        public static void SafeDispose(this object target)
        {
            var disposable = target as IDisposable;
            if (disposable == null) return;

            try
            {
                disposable.Dispose();
            }
            catch (Exception)
            {
            }
        }


        public static void TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
                                                Action<TValue> action)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
            {
                action(value);
            }
        }

        internal static T As<T>(this object target) where T : class
        {
            return target as T;
        }

        public static bool IsIn<T>(this T target, IList<T> list)
        {
            return list.Contains(target);
        }
    }

    namespace TypeRules
    {
        public static class TypeExtensions
        {

            public static bool Closes(this Type type, Type openType)
            {
                Type baseType = type.BaseType;
                if (baseType == null) return false;

                bool closes = baseType.IsGenericType && baseType.GetGenericTypeDefinition() == openType;
                if (closes) return true;

                return type.BaseType == null ? false : type.BaseType.Closes(openType);
            }

            public static bool IsInNamespace(this Type type, string nameSpace)
            {
                return type.Namespace != null && type.Namespace.StartsWith(nameSpace);
            }

            public static bool IsOpenGeneric(this Type type)
            {
                return type.IsGenericTypeDefinition || type.ContainsGenericParameters;
            }


            public static bool IsConcreteAndAssignableTo(this Type TPluggedType, Type pluginType)
            {
                return TPluggedType.IsConcrete() && pluginType.IsAssignableFrom(TPluggedType);
            }

            public static bool ImplementsInterfaceTemplate(this Type TPluggedType, Type templateType)
            {
                if (!TPluggedType.IsConcrete()) return false;

                foreach (Type interfaceType in TPluggedType.GetInterfaces())
                {
                    if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == templateType)
                    {
                        return true;
                    }
                }

                return false;
            }

            public static Type FindFirstInterfaceThatCloses(this Type TPluggedType, Type templateType)
            {
                return TPluggedType.FindInterfacesThatClose(templateType).FirstOrDefault();
            }

            public static IEnumerable<Type> FindInterfacesThatClose(this Type TPluggedType, Type templateType)
            {
                return rawFindInterfacesThatCloses(TPluggedType, templateType).Distinct();
            }

            private static IEnumerable<Type> rawFindInterfacesThatCloses(Type TPluggedType, Type templateType)
            {
                if (!TPluggedType.IsConcrete()) yield break;

                if (templateType.IsInterface)
                {
                    foreach (var interfaceType in TPluggedType.GetInterfaces().Where(type => type.IsGenericType && (type.GetGenericTypeDefinition() == templateType)))
                    {
                        yield return interfaceType;
                    }
                }
                else if (TPluggedType.BaseType.IsGenericType && (TPluggedType.BaseType.GetGenericTypeDefinition() == templateType))
                {
                    yield return TPluggedType.BaseType;
                }

                if (TPluggedType.BaseType == typeof (object)) yield break;

                foreach (var interfaceType in rawFindInterfacesThatCloses(TPluggedType.BaseType, templateType))
                {
                    yield return interfaceType;
                }
            }

            public static bool IsNullable(this Type type)
            {
                return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
            }

            public static Type GetInnerTypeFromNullable(this Type nullableType)
            {
                return nullableType.GetGenericArguments()[0];
            }

            public static string GetName(this Type type)
            {
                if (type.IsGenericType)
                {
                    string[] parameters = Array.ConvertAll(type.GetGenericArguments(), t => t.GetName());
                    string parameterList = String.Join(", ", parameters);
                    return "{0}<{1}>".ToFormat(type.Name, parameterList);
                }

                return type.Name;
            }

            public static string GetFullName(this Type type)
            {
                if (type.IsGenericType)
                {
                    string[] parameters = Array.ConvertAll(type.GetGenericArguments(), t => t.GetName());
                    string parameterList = String.Join(", ", parameters);
                    return "{0}<{1}>".ToFormat(type.Name, parameterList);
                }

                return type.FullName;
            }

            public static bool CanBeCreated(this Type type)
            {
                return type.IsConcrete() && Constructor.HasConstructors(type);
            }

            public static IEnumerable<Type> AllInterfaces(this Type type)
            {
                foreach (Type @interface in type.GetInterfaces())
                {
                    yield return @interface;
                }
            }

            /// <summary>
            /// Determines if the TPluggedType can be upcast to the pluginType
            /// </summary>
            /// <param name="pluginType"></param>
            /// <param name="TPluggedType"></param>
            /// <returns></returns>
            public static bool CanBeCastTo(this Type TPluggedType, Type pluginType)
            {
                if (TPluggedType == null) return false;

                if (TPluggedType.IsInterface || TPluggedType.IsAbstract)
                {
                    return false;
                }

                if (pluginType.IsOpenGeneric())
                {
                    return GenericsPluginGraph.CanBeCast(pluginType, TPluggedType);
                }

                if (IsOpenGeneric(TPluggedType))
                {
                    return false;
                }


                return pluginType.IsAssignableFrom(TPluggedType);
            }


            /// <summary>
            /// Determines if the TPluggedType is a valid Plugin into the
            /// PluginType
            /// </summary>
            /// <param name="pluginType"></param>
            /// <param name="TPluggedType"></param>
            /// <returns></returns>
            public static bool IsExplicitlyMarkedAsPlugin(this Type TPluggedType, Type pluginType)
            {
                bool returnValue = false;

                bool markedAsPlugin = PluggableAttribute.MarkedAsPluggable(TPluggedType);
                if (markedAsPlugin)
                {
                    returnValue = CanBeCastTo(TPluggedType, pluginType);
                }

                return returnValue;
            }

            public static bool IsString(this Type type)
            {
                return type.Equals(typeof (string));
            }

            public static bool IsPrimitive(this Type type)
            {
                return type.IsPrimitive && !IsString(type) && type != typeof (IntPtr);
            }

            public static bool IsSimple(this Type type)
            {
                return type.IsPrimitive || IsString(type) || type.IsEnum;
            }

            public static bool IsChild(this Type type)
            {
                return IsPrimitiveArray(type) || (!type.IsArray && !IsSimple(type));
            }

            public static bool IsChildArray(this Type type)
            {
                return type.IsArray && !IsSimple(type.GetElementType());
            }

            public static bool IsPrimitiveArray(this Type type)
            {
                return type.IsArray && IsSimple(type.GetElementType());
            }

            public static bool IsConcrete(this Type type)
            {
                return !type.IsAbstract && !type.IsInterface;
            }

            public static bool IsAutoFillable(this Type type)
            {
                return IsChild(type) || IsChildArray(type);
            }
        }
    }
}
