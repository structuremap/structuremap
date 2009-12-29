using System;
using System.Collections.Generic;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap
{
    public static class BasicExtensions
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


        public static void TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
                                                Action<TValue> action)
        {
            TValue value;
            if (dictionary.TryGetValue(key, out value))
            {
                action(value);
            }
        }

        public static T As<T>(this object target) where T : class
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
                return type.Namespace.StartsWith(nameSpace);
            }

            public static ReferencedInstance GetReferenceTo(this Type type)
            {
                string key = PluginCache.GetPlugin(type).ConcreteKey;
                return new ReferencedInstance(key);
            }

            public static bool IsOpenGeneric(this Type type)
            {
                return type.IsGenericTypeDefinition || type.ContainsGenericParameters;
            }


            public static bool IsConcreteAndAssignableTo(this Type pluggedType, Type pluginType)
            {
                return pluggedType.IsConcrete() && pluginType.IsAssignableFrom(pluggedType);
            }

            public static bool ImplementsInterfaceTemplate(this Type pluggedType, Type templateType)
            {
                if (!pluggedType.IsConcrete()) return false;

                foreach (Type interfaceType in pluggedType.GetInterfaces())
                {
                    if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == templateType)
                    {
                        return true;
                    }
                }

                return false;
            }

            public static Type FindInterfaceThatCloses(this Type pluggedType, Type templateType)
            {
                if (!pluggedType.IsConcrete()) return null;

                if (templateType.IsInterface)
                {
                    foreach (Type interfaceType in pluggedType.GetInterfaces())
                    {
                        if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == templateType)
                        {
                            return interfaceType;
                        }
                    }
                }
                else if (pluggedType.BaseType.IsGenericType &&
                         pluggedType.BaseType.GetGenericTypeDefinition() == templateType)
                {
                    return pluggedType.BaseType;
                }

                return pluggedType.BaseType == typeof (object)
                           ? null
                           : pluggedType.BaseType.FindInterfaceThatCloses(templateType);
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
            /// Determines if the pluggedType can be upcast to the pluginType
            /// </summary>
            /// <param name="pluginType"></param>
            /// <param name="pluggedType"></param>
            /// <returns></returns>
            public static bool CanBeCastTo(this Type pluggedType, Type pluginType)
            {
                if (pluggedType == null) return false;

                if (pluggedType.IsInterface || pluggedType.IsAbstract)
                {
                    return false;
                }

                if (pluginType.IsOpenGeneric() && pluggedType.IsGenericType)
                {
                    return GenericsPluginGraph.CanBeCast(pluginType, pluggedType);
                }

                if (IsOpenGeneric(pluggedType))
                {
                    return false;
                }


                return pluginType.IsAssignableFrom(pluggedType);
            }


            /// <summary>
            /// Determines if the PluggedType is a valid Plugin into the
            /// PluginType
            /// </summary>
            /// <param name="pluginType"></param>
            /// <param name="pluggedType"></param>
            /// <returns></returns>
            public static bool IsExplicitlyMarkedAsPlugin(this Type pluggedType, Type pluginType)
            {
                bool returnValue = false;

                bool markedAsPlugin = PluggableAttribute.MarkedAsPluggable(pluggedType);
                if (markedAsPlugin)
                {
                    returnValue = CanBeCastTo(pluggedType, pluginType);
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