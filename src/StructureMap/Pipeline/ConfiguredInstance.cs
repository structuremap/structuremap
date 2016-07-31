using System;
using System.Collections.Generic;
using System.Reflection;
using StructureMap.Building.Interception;

namespace StructureMap.Pipeline
{
    /// <summary>
    /// An Instance class that builds objects by calling a constructor function on a concrete type
    /// and filling setter properties.  ConfiguredInstance should only be used for open generic types.
    /// Favor <see cref="SmartInstance{T}">SmartInstance{T}</see> for all other usages.
    /// </summary>
    public class ConfiguredInstance : ConstructorInstance<ConfiguredInstance>
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

        public ConfiguredInstance(Type pluggedType, string name, DependencyCollection dependencies, IEnumerable<IInterceptor> interceptors, ConstructorInfo constructor) : base(pluggedType)
        {
            Name = name;
            Constructor = constructor;
            interceptors.Each(AddInterceptor);
            dependencies.CopyTo(Dependencies);
        }

        protected override ConfiguredInstance thisObject()
        {
            return this;
        }

        protected override ConfiguredInstance thisInstance => this;
    }
}