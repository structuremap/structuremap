using System;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public partial class ConfiguredInstance : ConfiguredInstanceBase<ConfiguredInstance>
    {
        public ConfiguredInstance(InstanceMemento memento, PluginGraph graph, Type pluginType)
            : base(memento, graph, pluginType)
        {
        }

        public ConfiguredInstance(Type pluggedType, string name) : base(pluggedType, name)
        {
        }


        public ConfiguredInstance(Type pluggedType) : base(pluggedType)
        {
        }

        public ConfiguredInstance(Type templateType, params Type[] types) : base(GetGenericType(templateType, types))
        {
        }

        public static Type GetGenericType(Type templateType, params Type[] types)
        {
            return templateType.MakeGenericType(types);
        }

        protected void setPluggedType(Type pluggedType)
        {
            _pluggedType = pluggedType;
        }


        protected override void preprocess(PluginFamily family)
        {
        }

        protected override string getDescription()
        {
            string typeName = _pluggedType.AssemblyQualifiedName;
            var ctor = new Constructor(_pluggedType);
            if (ctor.HasArguments())
            {
                return "Configured " + typeName;
            }
            else
            {
                return typeName;
            }
        }

        protected override void addTemplatedInstanceTo(PluginFamily family, Type[] templateTypes)
        {
            Type specificType = _pluggedType.IsGenericTypeDefinition
                                    ? _pluggedType.MakeGenericType(templateTypes)
                                    : _pluggedType;
            if (TypeRules.CanBeCast(family.PluginType, specificType))
            {
                var instance = new ConfiguredInstance(specificType);
                instance._arrays = _arrays;
                instance._children = _children;
                instance._properties = _properties;
                instance.Name = Name;
                family.AddInstance(instance);
            }
        }
    }
}