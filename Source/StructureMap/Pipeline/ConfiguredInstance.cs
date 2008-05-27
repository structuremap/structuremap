using System;
using System.Collections.Generic;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public partial class ConfiguredInstance : ExpressedInstance<ConfiguredInstance>, IConfiguredInstance,
                                              IStructuredInstance
    {
        private readonly Dictionary<string, Instance> _children = new Dictionary<string, Instance>();
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();
        private Dictionary<string, Instance[]> _arrays = new Dictionary<string, Instance[]>();

        private string _concreteKey;
        private Type _pluggedType;


        public ConfiguredInstance()
        {
        }

        public ConfiguredInstance(InstanceMemento memento, PluginGraph graph, Type pluginType)
        {
            read(memento, graph, pluginType);
        }

        public ConfiguredInstance(string name)
        {
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
            _concreteKey = instance._concreteKey;

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
            InstanceBuilder builder = session.FindBuilderByType(pluginType, _pluggedType) ??
                                      session.FindBuilderByConcreteKey(pluginType, _concreteKey);

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
            // F-ing generics.  You have to check concrete key first
            if (!string.IsNullOrEmpty(_concreteKey))
            {
                return family.Plugins.HasPlugin(_concreteKey);
            }

            if (_pluggedType != null)
            {
                return TypeRules.CanBeCast(family.PluginType, _pluggedType);
            }

            return false;
        }

        internal override bool Matches(Plugin plugin)
        {
            return plugin.ConcreteKey == _concreteKey || plugin.PluggedType == _pluggedType;
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
            _concreteKey = plugin.ConcreteKey;

            InstanceMementoPropertyReader reader = new InstanceMementoPropertyReader(this, memento, graph, pluginType);
            plugin.VisitArguments(reader);
        }


        private void setChild(string name, Instance instance)
        {
            _children.Add(name, instance);
        }

        private void setChildArray(string name, Instance[] array)
        {
            _arrays.Add(name, array);
        }


        protected override void preprocess(PluginFamily family)
        {
            if (_pluggedType != null)
            {
                Plugin plugin = family.FindPlugin(_pluggedType);
                _concreteKey = plugin.ConcreteKey;
            }
        }

        protected override string getDescription()
        {
            if (_pluggedType == null)
            {
                return string.Format("Configured '{0}'", _concreteKey);
            }

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
    }
}