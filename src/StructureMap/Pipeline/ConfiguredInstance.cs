using System;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    /// <summary>
    /// An Instance class that builds objects by calling a constructor function on a concrete type
    /// and filling setter properties.  ConfiguredInstance should only be used for open generic types.
    /// Favor <see cref="SmartInstance{T}">SmartInstance{T}</see> for all other usages.
    /// </summary>
    public partial class ConfiguredInstance : ConstructorInstance<ConfiguredInstance>
    {


        public ConfiguredInstance(Type pluggedType, string name)
            : base(pluggedType, name)
        {
        }


        public ConfiguredInstance(Type pluggedType)
            : base(pluggedType)
        {
        }

        public ConfiguredInstance(Type templateType, params Type[] types)
            : base(templateType.MakeGenericType(types))
        {
        }

        protected override ConfiguredInstance thisObject()
        {
            return this;
        }
    }
}