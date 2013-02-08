using System;
using StructureMap.Graph;
using System.Linq;

namespace StructureMap
{
    /// <summary>
    /// Declares a class, abstract class, or interface to be the target of a PluginFamily in the container
    /// </summary>
    public class PluginFamilyAttribute : FamilyAttribute
    {
        private readonly string _default = string.Empty;
        private InstanceScope _scope = InstanceScope.PerRequest;

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
        public InstanceScope Scope { get { return _scope; } set { _scope = value; } }

        /// <summary>
        /// Declares the target to be built by StructureMap as a Singleton.  One object instance will
        /// be created for each named instance
        /// </summary>
        public bool IsSingleton { get { return _scope == InstanceScope.Singleton; } set { _scope = value ? InstanceScope.Singleton : InstanceScope.PerRequest; } }


        /// <summary>
        /// Determines if a Type object is marked as a PluginFamily
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static bool MarkedAsPluginFamily(Type objectType)
        {
            var att =
                GetCustomAttribute(objectType, typeof (PluginFamilyAttribute), false) as PluginFamilyAttribute;
            return (att != null);
        }

        public static void ConfigureFamily(IPluginFamily family)
        {
            var att =
                GetCustomAttribute(family.PluginType, typeof (PluginFamilyAttribute), false)
                as PluginFamilyAttribute;

            if (att != null)
            {
                att.Configure(family);
            }
        }

        public override void Alter(PluginFamily family)
        {
            Configure(family);
        }

        public void Configure(IPluginFamily family)
        {
            if (Scope != InstanceScope.PerRequest) family.SetScopeTo(Scope);
        }
    }
}