using System;
using System.Collections.Generic;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    public abstract class ConfiguredInstanceBase<T> : Instance, IConfiguredInstance, IStructuredInstance
    {
        protected Dictionary<string, Instance> _children = new Dictionary<string, Instance>();
        protected Dictionary<string, string> _properties = new Dictionary<string, string>();
        protected Dictionary<string, Instance[]> _arrays = new Dictionary<string, Instance[]>();
        protected Type _pluggedType;

        protected ConfiguredInstanceBase(InstanceMemento memento, PluginGraph graph, Type pluginType)
        {
            read(memento, graph, pluginType);
        }

        protected ConfiguredInstanceBase(Type pluggedType, string name)
        {
            // TODO -- need defensive check HERE!
            //throw new NotImplementedException("Need to check for public constructor HERE!");

            _pluggedType = pluggedType;
            Name = name;
        }

        protected ConfiguredInstanceBase(Type pluggedType) : this(pluggedType, Guid.NewGuid().ToString())
        {
        }


        Type IConfiguredInstance.PluggedType
        {
            get { return _pluggedType; }
        }

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

        protected override object build(Type pluginType, BuildSession session)
        {
            InstanceBuilder builder = PluginCache.FindBuilder(_pluggedType);
            return ((IConfiguredInstance) this).Build(pluginType, session, builder);
        }

        protected virtual object getChild(string propertyName, Type pluginType, BuildSession buildSession)
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

        private void read(InstanceMemento memento, PluginGraph graph, Type pluginType)
        {
            PluginFamily family = graph.FindFamily(pluginType);
            Plugin plugin = memento.FindPlugin(family);

            _pluggedType = plugin.PluggedType;

            InstanceMementoPropertyReader reader = new InstanceMementoPropertyReader(this, memento, graph, pluginType);
            plugin.VisitArguments(reader);
        }

        protected void setChild(string name, Instance instance)
        {
            if (instance == null) return;

            _children.Add(name, instance);
        }

        protected void setChildArray(string name, Instance[] array)
        {
            _arrays.Add(name, array);
        }

        Instance[] IConfiguredInstance.GetChildrenArray(string propertyName)
        {
            return _arrays.ContainsKey(propertyName) ? _arrays[propertyName] : null;
        }

        string IConfiguredInstance.GetProperty(string propertyName)
        {
            if (!_properties.ContainsKey(propertyName))
            {
                throw new StructureMapException(205, propertyName, Name);
            }

            return _properties[propertyName];
        }

        object IConfiguredInstance.GetChild(string propertyName, Type pluginType, BuildSession buildSession)
        {
            return getChild(propertyName, pluginType, buildSession);
        }

        object IConfiguredInstance.Build(Type pluginType, BuildSession session, InstanceBuilder builder)
        {
            if (builder == null)
            {
                throw new StructureMapException(
                    201, _pluggedType.FullName, Name, pluginType);
            }


            try
            {
                return builder.BuildInstance(this, session);
            }
            catch (StructureMapException)
            {
                throw;
            }
            catch (InvalidCastException ex)
            {
                throw new StructureMapException(206, ex, Name);
            }
            catch (Exception ex)
            {
                throw new StructureMapException(207, ex, Name, pluginType.FullName);
            }
        }

        bool IConfiguredInstance.HasProperty(string propertyName)
        {
            return _properties.ContainsKey(propertyName) || _children.ContainsKey(propertyName) || _arrays.ContainsKey(propertyName);
        }

        protected override Type getConcreteType(Type pluginType)
        {
            return _pluggedType;
        }

        protected string findPropertyName<T>()
        {
            Plugin plugin = new Plugin(_pluggedType);
            string propertyName = plugin.FindArgumentNameForType<T>();

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new StructureMapException(305, typeof (T));
            }

            return propertyName;
        }

        protected static void validateTypeIsArray<PLUGINTYPE>()
        {
            if (!typeof (PLUGINTYPE).IsArray)
            {
                throw new StructureMapException(307);
            }
        }

        void IConfiguredInstance.SetProperty(string propertyName, string propertyValue)
        {
            setProperty(propertyName, propertyValue);
        }

        protected void setProperty(string propertyName, string propertyValue)
        {
            _properties[propertyName] = propertyValue;
        }

        void IConfiguredInstance.SetChild(string name, Instance instance)
        {
            setChild(name, instance);
        }

        void IConfiguredInstance.SetChildArray(string name, Type type, Instance[] children)
        {
            setChildArray(name, children);
        }
    }
}