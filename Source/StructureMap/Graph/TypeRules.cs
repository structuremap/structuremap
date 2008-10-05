using System;

namespace StructureMap.Graph
{
    public class TypeRules
    {
        /// <summary>
        /// Determines if the pluggedType can be upcast to the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="pluggedType"></param>
        /// <returns></returns>
        public static bool CanBeCast(Type pluginType, Type pluggedType)
        {
            if (pluggedType.IsInterface || pluggedType.IsAbstract)
            {
                return false;
            }

            if (noPublicConstructors(pluggedType))
            {
                return false;
            }

            if (GenericsPluginGraph.CanBeCast(pluginType, pluggedType))
            {
                return true;
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
        public static bool IsExplicitlyMarkedAsPlugin(Type pluginType, Type pluggedType)
        {
            bool returnValue = false;

            bool markedAsPlugin = PluggableAttribute.MarkedAsPluggable(pluggedType);
            if (markedAsPlugin)
            {
                returnValue = CanBeCast(pluginType, pluggedType);
            }

            return returnValue;
        }

        private static bool noPublicConstructors(Type pluggedType)
        {
            return pluggedType.GetConstructors().Length == 0;
        }

        protected bool IsString(Type type)
        {
            return type.Equals(typeof (string));
        }

        protected bool IsPrimitive(Type type)
        {
            return type.IsPrimitive && !IsString(type) && type != typeof (IntPtr);
        }

        protected bool IsSimple(Type type)
        {
            return type.IsPrimitive || IsString(type) || IsEnum(type);
        }

        protected bool IsEnum(Type type)
        {
            return type.IsEnum;
        }

        protected bool IsChild(Type type)
        {
            return IsPrimitiveArray(type) || (!type.IsArray && !IsSimple(type));
        }

        protected bool IsChildArray(Type type)
        {
            return type.IsArray && !IsSimple(type.GetElementType());
        }

        protected bool IsPrimitiveArray(Type type)
        {
            return type.IsArray && IsSimple(type.GetElementType());
        }

        protected bool IsConcrete(Type type)
        {
            return !type.IsInterface && !type.IsAbstract;
        }


        protected bool IsAutoFillable(Type type)
        {
            return IsChild(type) || IsChildArray(type);
        }
    }
}