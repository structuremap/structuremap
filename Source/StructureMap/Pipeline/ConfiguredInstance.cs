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


        protected override ConfiguredInstance thisInstance
        {
            get { return this; }
        }

        #region IStructuredInstance Members

        #endregion

        protected void setPluggedType(Type pluggedType)
        {
            _pluggedType = pluggedType;
        }


        protected void mergeIntoThis(ConfiguredInstance instance)
        {
            _pluggedType = instance._pluggedType;

            foreach (KeyValuePair<string, string> pair in instance._properties)
            {
                if (!_properties.ContainsKey(pair.Key))
                {
                    _properties.Add(pair.Key, pair.Value);
                }
            }

            foreach (KeyValuePair<string, Instance> pair in instance._children)
            {
                if (!_children.ContainsKey(pair.Key))
                {
                    _children.Add(pair.Key, pair.Value);
                }
            }

            _arrays = instance._arrays;
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