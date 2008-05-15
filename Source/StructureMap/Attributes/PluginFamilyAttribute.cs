using System;
using StructureMap.Attributes;
using StructureMap.Graph;

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

        public static void ConfigureFamily(IPluginFamily family)
        {
            PluginFamilyAttribute att =
                GetCustomAttribute(family.PluginType, typeof (PluginFamilyAttribute), false)
                as PluginFamilyAttribute;

            if (att != null)
            {
                att.Configure(family);
            }
        }

        public void Configure(IPluginFamily family)
        {
            if (SourceType != null)
            {
                try
                {
                    MementoSource source = (MementoSource) Activator.CreateInstance(SourceType);
                    family.AddMementoSource(source);
                }
                catch (Exception ex)
                {
                    throw new StructureMapException(122, ex, SourceType.FullName,
                                                    family.PluginType.AssemblyQualifiedName);
                }
            }


            if (!string.IsNullOrEmpty(DefaultKey)) family.DefaultInstanceKey = DefaultKey;
            if (Scope != InstanceScope.PerRequest) family.SetScopeTo(Scope);
        }
    }
}