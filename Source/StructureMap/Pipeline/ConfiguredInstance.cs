using System;
using System.Collections.Generic;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public partial class ConfiguredInstance : ConfiguredInstanceBase<ConfiguredInstance>
    {
        public ConfiguredInstance(InstanceMemento memento, PluginGraph graph, Type pluginType) : base(memento, graph, pluginType)
        {
        }

        public ConfiguredInstance(Type pluggedType, string name) : base(pluggedType, name)
        {
        }


        public ConfiguredInstance(Type pluggedType) : base(pluggedType)
        {
        }

        #region IStructuredInstance Members

        #endregion

        protected void setPluggedType(Type pluggedType)
        {
            _pluggedType = pluggedType;
        }


        protected override void preprocess(PluginFamily family)
        {

        }

        protected override string getDescription()
        {
            string typeName = TypePath.GetAssemblyQualifiedName(_pluggedType);
            Constructor ctor = new Constructor(_pluggedType);
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
            Type specificType = _pluggedType.IsGenericTypeDefinition ? _pluggedType.MakeGenericType(templateTypes) : _pluggedType;
            if (TypeRules.CanBeCast(family.PluginType, specificType))
            {
                ConfiguredInstance instance = new ConfiguredInstance(specificType);
                instance._arrays = _arrays;
                instance._children = _children;
                instance._properties = _properties;
                instance.Name = Name;
                family.AddInstance(instance);
            }
        }
    }
}