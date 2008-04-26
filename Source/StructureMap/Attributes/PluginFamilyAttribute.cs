using System;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Source;

namespace StructureMap
{
    /// <summary>
    /// Declares a class, abstract class, or interface to be the target of a PluginFamily in the container
    /// </summary>
    public class PluginFamilyAttribute : Attribute
    {
        private string _default = string.Empty;
        private InstanceScope _scope = InstanceScope.PerRequest;
        private Type _source = null;

        public PluginFamilyAttribute()
        {
        }


        public PluginFamilyAttribute(string DefaultKey)
        {
            _default = DefaultKey;
        }

        /// <summary>
        /// If set, determines the shared "scope" of the instance -- PerRequest, Singleton, ThreadLocal,
        /// HttpContext, etc.
        /// </summary>
        public InstanceScope Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }

        public Type SourceType
        {
            get { return _source; }
            set { _source = value; }
        }

        /// <summary>
        /// InstanceKey of the default instance.  Used to implicitly define the default without
        /// declaring the instance in StructureMap.config
        /// </summary>
        public string DefaultKey
        {
            get { return _default; }
        }

        /// <summary>
        /// Declares the target to be built by StructureMap as a Singleton.  One object instance will
        /// be created for each named instance
        /// </summary>
        public bool IsSingleton
        {
            get { return _scope == InstanceScope.Singleton; }
            set { _scope = value ? InstanceScope.Singleton : InstanceScope.PerRequest; }
        }

        public MementoSource CreateSource(Type exportedType)
        {
            if (SourceType != null)
            {
                try
                {
                    return (MementoSource) Activator.CreateInstance(SourceType);
                }
                catch (Exception ex)
                {
                    throw new StructureMapException(122, ex, SourceType.FullName, exportedType.FullName);
                }
            }
            else
            {
                return new MemoryMementoSource();
            }
        }

        /// <summary>
        /// Determines if a Type object is marked as a PluginFamily
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static bool MarkedAsPluginFamily(Type objectType)
        {
            PluginFamilyAttribute att =
                GetCustomAttribute(objectType, typeof (PluginFamilyAttribute), false) as PluginFamilyAttribute;
            return (att != null);
        }

        /// <summary>
        /// Gets the default instance key from a Type marked as a PluginFamily
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static string GetDefaultKey(Type objectType)
        {
            PluginFamilyAttribute att =
                GetCustomAttribute(objectType, typeof (PluginFamilyAttribute), false) as PluginFamilyAttribute;
            if (att == null)
            {
                return string.Empty;
            }
            else
            {
                return att.DefaultKey;
            }
        }

        /// <summary>
        /// Interrogates the attribute on the pluginType and determines if the PluginFamily is
        /// marked as a Singleton
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public static bool IsMarkedAsSingleton(Type pluginType)
        {
            bool returnValue = false;

            PluginFamilyAttribute att =
                GetCustomAttribute(
                    pluginType,
                    typeof (PluginFamilyAttribute), false)
                as PluginFamilyAttribute;


            if (att != null)
            {
                returnValue = att.IsSingleton;
            }

            return returnValue;
        }

        public PluginFamily BuildPluginFamily(Type exportedType)
        {
            if (!MarkedAsPluginFamily(exportedType))
            {
                return new PluginFamily(exportedType);
            }

            MementoSource source = CreateSource(exportedType);
            PluginFamily family = new PluginFamily(exportedType, DefaultKey);
            family.AddMementoSource(source);

            family.SetScopeTo(Scope);

            return family;
        }

        public static PluginFamily CreatePluginFamily(Type exportedType)
        {
            PluginFamilyAttribute att = GetAttribute(exportedType);
            PluginFamily family = att.BuildPluginFamily(exportedType);

            return family;
        }

        public static PluginFamilyAttribute GetAttribute(Type exportedType)
        {
            return GetCustomAttribute(exportedType, typeof (PluginFamilyAttribute), false) as PluginFamilyAttribute;
        }
    }
}