using System;
using System.Collections.Generic;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public partial class ConfiguredInstance : ExpressedInstance<ConfiguredInstance>, IConfiguredInstance,
                                              IStructuredInstance
    {
        private Dictionary<string, Instance> _children = new Dictionary<string, Instance>();
        private Dictionary<string, string> _properties = new Dictionary<string, string>();
        private Dictionary<string, Instance[]> _arrays = new Dictionary<string, Instance[]>();

        private Type _pluggedType;


        public ConfiguredInstance(InstanceMemento memento, PluginGraph graph, Type pluginType)
        {
            read(memento, graph, pluginType);
        }

        public ConfiguredInstance(Type pluggedType, string name)
        {
            _pluggedType = pluggedType;
            Name = name;
        }


        public ConfiguredInstance(Type pluggedType)
        {
            _pluggedType = pluggedType;
        }


        protected override ConfiguredInstance thisInstance
        {
            get { return this; }
        }

        #region IStructuredInstance Members

        Instance IStructuredInstance.GetChild(string name)
        {
            return _children[name];
        }

        Instance[] IStructuredInstance.GetChildArray(string name)
        {
            return _arrays[name];
        }

        void IStructuredInstance.RemoveKey(string name)
        {
            _properties.Remove(name);
        }

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

        protected override object build(Type pluginType, IBuildSession session)
        {
            InstanceBuilder builder = PluginCache.FindBuilder(_pluggedType);
            return ((IConfiguredInstance) this).Build(pluginType, session, builder);
        }

        protected virtual object getChild(string propertyName, Type pluginType, IBuildSession buildSession)
        {
            Instance childInstance = _children.ContainsKey(propertyName)
                                         ? _children[propertyName]
                                         : new DefaultInstance();


            return childInstance.Build(pluginType, buildSession);
        }


        protected override bool canBePartOfPluginFamily(PluginFamily family)
        {
             return TypeRules.CanBeCast(family.PluginType, _pluggedType);
        }

        internal override bool Matches(Plugin plugin)
        {
            return plugin.PluggedType == _pluggedType;
        }

        public ConfiguredInstance SetProperty(string propertyName, string propertyValue)
        {
            _properties[propertyName] = propertyValue;
            return this;
        }

        private void read(InstanceMemento memento, PluginGraph graph, Type pluginType)
        {
            PluginFamily family = graph.FindFamily(pluginType);
            Plugin plugin = memento.FindPlugin(family);

            _pluggedType = plugin.PluggedType;

            InstanceMementoPropertyReader reader = new InstanceMementoPropertyReader(this, memento, graph, pluginType);
            plugin.VisitArguments(reader);
        }


        private void setChild(string name, Instance instance)
        {
            if (instance == null) return;

            _children.Add(name, instance);
        }

        private void setChildArray(string name, Instance[] array)
        {
            _arrays.Add(name, array);
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