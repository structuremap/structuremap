using System;
using System.Collections.Generic;
using System.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StructureMap.Pipeline
{
    // TODO:  Move the FI stuff into a partial class
    public class ConfiguredInstance : ExpressedInstance<ConfiguredInstance>, IConfiguredInstance
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
            Read(memento, graph, pluginType);
        }

        public ConfiguredInstance(string name)
        {
            Name = name;
        }


        public ConfiguredInstance(Type pluggedType)
        {
            _pluggedType = pluggedType;
        }

        public Type PluggedType
        {
            get { return _pluggedType; }
            set { _pluggedType = value; }
        }


        protected override ConfiguredInstance thisInstance
        {
            get { return this; }
        }

        #region IConfiguredInstance Members

        public string ConcreteKey
        {
            get
            {
                return _concreteKey;
            }
            set { _concreteKey = value; }
        }

        Instance[] IConfiguredInstance.GetChildrenArray(string propertyName)
        {
            if (_arrays.ContainsKey(propertyName))
            {
                return _arrays[propertyName];
            }

            return null;
        }

        string IConfiguredInstance.GetProperty(string propertyName)
        {
            if (!_properties.ContainsKey(propertyName))
            {
                throw new StructureMapException(205, propertyName, Name);
            }

            return _properties[propertyName];
        }

        object IConfiguredInstance.GetChild(string propertyName, Type pluginType, IBuildSession buildSession)
        {
            return getChild(propertyName, pluginType, buildSession);
        }

        #endregion

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
                                      session.FindBuilderByConcreteKey(pluginType, ConcreteKey);

            return Build(pluginType, session, builder);
        }

        // Only open for testing
        public object Build(Type pluginType, IBuildSession session, InstanceBuilder builder)
        {
            if (builder == null)
            {
                throw new StructureMapException(
                    201, ConcreteKey, Name, pluginType);
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

            if (PluggedType != null)
            {
                return TypeRules.CanBeCast(family.PluginType, PluggedType);
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

        public void Read(InstanceMemento memento, PluginGraph graph, Type pluginType)
        {
            PluginFamily family = graph.FindFamily(pluginType);
            Plugin plugin = memento.FindPlugin(family);

            PluggedType = plugin.PluggedType;
            _concreteKey = plugin.ConcreteKey;

            InstanceMementoPropertyReader reader = new InstanceMementoPropertyReader(this, memento, graph, pluginType);
            plugin.VisitArguments(reader);
        }


        public void SetChild(string name, Instance instance)
        {
            _children.Add(name, instance);
        }

        public Instance GetChild(string name)
        {
            return _children[name];
        }

        public void SetChildArray(string name, Instance[] array)
        {
            _arrays.Add(name, array);
        }


        public Instance[] GetChildArray(string name)
        {
            return _arrays[name];
        }

        public ConfiguredInstance WithConcreteKey(string concreteKey)
        {
            replaceNameIfNotAlreadySet(concreteKey);
            _concreteKey = concreteKey;
            return this;
        }

        public void RemoveKey(string name)
        {
            _properties.Remove(name);
        }

        /// <summary>
        /// Start the definition of a primitive argument to a constructor argument
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyExpression WithProperty(string propertyName)
        {
            return new PropertyExpression(this, propertyName);
        }

        /// <summary>
        /// Starts the definition of a child instance specifying the argument name
        /// in the case of a constructor function that consumes more than one argument
        /// of type T
        /// </summary>
        /// <typeparam name="CONSTRUCTORARGUMENTTYPE"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public ChildInstanceExpression Child<CONSTRUCTORARGUMENTTYPE>(string propertyName)
        {
            ChildInstanceExpression child = new ChildInstanceExpression(this, propertyName);
            child.ChildType = typeof (CONSTRUCTORARGUMENTTYPE);

            return child;
        }

        /// <summary>
        /// Start the definition of a child instance for type CONSTRUCTORARGUMENTTYPE
        /// </summary>
        /// <typeparam name="CONSTRUCTORARGUMENTTYPE"></typeparam>
        /// <returns></returns>
        public ChildInstanceExpression Child<CONSTRUCTORARGUMENTTYPE>()
        {
            string propertyName = findPropertyName<CONSTRUCTORARGUMENTTYPE>();

            ChildInstanceExpression child = new ChildInstanceExpression(this, propertyName);
            child.ChildType = typeof (CONSTRUCTORARGUMENTTYPE);
            return child;
        }

        private string findPropertyName<T>()
        {
            Plugin plugin = new Plugin(_pluggedType);
            string propertyName = plugin.FindFirstConstructorArgumentOfType<T>();

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new StructureMapException(305, typeof (T));
            }

            return propertyName;
        }

        public ChildArrayExpression<PLUGINTYPE> ChildArray<PLUGINTYPE>()
        {
            validateTypeIsArray<PLUGINTYPE>();

            string propertyName = findPropertyName<PLUGINTYPE>();
            return ChildArray<PLUGINTYPE>(propertyName);
        }

        public ChildArrayExpression<PLUGINTYPE> ChildArray<PLUGINTYPE>(string propertyName)
        {
            validateTypeIsArray<PLUGINTYPE>();

            ChildArrayExpression<PLUGINTYPE> expression =
                new ChildArrayExpression<PLUGINTYPE>(this, propertyName);

            return expression;
        }

        private static void validateTypeIsArray<PLUGINTYPE>()
        {
            if (!typeof (PLUGINTYPE).IsArray)
            {
                throw new StructureMapException(307);
            }
        }

        /// <summary>
        /// Use type T for the concrete type of an instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ConfiguredInstance UsingConcreteType<T>()
        {
            _pluggedType = typeof (T);
            return this;
        }

        /// <summary>
        /// Use a named Plugin type denoted by a [Pluggable("Key")] attribute
        /// </summary>
        /// <param name="concreteKey"></param>
        /// <returns></returns>
        public ConfiguredInstance UsingConcreteTypeNamed(string concreteKey)
        {
            _concreteKey = concreteKey;
            return this;
        }

        #region Nested type: ChildArrayExpression

        public class ChildArrayExpression<PLUGINTYPE>
        {
            private readonly ConfiguredInstance _instance;
            private readonly string _propertyName;
            private Type _pluginType = typeof (PLUGINTYPE);

            public ChildArrayExpression(ConfiguredInstance instance, string propertyName)
            {
                _instance = instance;
                _propertyName = propertyName;

                _pluginType = typeof (PLUGINTYPE).GetElementType();
            }

            public ConfiguredInstance Contains(params Instance[] instances)
            {
                _instance.SetChildArray(_propertyName, instances);

                return _instance;
            }
        }

        #endregion

        #region Nested type: ChildInstanceExpression

        /// <summary>
        /// Part of the Fluent Interface, represents a nonprimitive argument to a 
        /// constructure function
        /// </summary>
        public class ChildInstanceExpression
        {
            private readonly ConfiguredInstance _instance;
            private readonly string _propertyName;
            private Type _childType;


            public ChildInstanceExpression(ConfiguredInstance instance, string propertyName)
            {
                _instance = instance;
                _propertyName = propertyName;
            }

            public ChildInstanceExpression(ConfiguredInstance instance, string propertyName,
                                           Type childType)
                : this(instance, propertyName)
            {
                _childType = childType;
            }

            internal Type ChildType
            {
                set { _childType = value; }
            }

            /// <summary>
            /// Use a previously configured and named instance for the child
            /// </summary>
            /// <param name="instanceKey"></param>
            /// <returns></returns>
            public ConfiguredInstance IsNamedInstance(string instanceKey)
            {
                ReferencedInstance instance = new ReferencedInstance(instanceKey);
                _instance.SetChild(_propertyName, instance);

                return _instance;
            }

            /// <summary>
            /// Start the definition of a child instance by defining the concrete type
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public ConfiguredInstance IsConcreteType<T>()
            {
                Type pluggedType = typeof (T);
                ExpressionValidator.ValidatePluggabilityOf(pluggedType).IntoPluginType(_childType);

                ConfiguredInstance childInstance = new ConfiguredInstance();
                childInstance.PluggedType = pluggedType;
                _instance.SetChild(_propertyName, childInstance);

                return _instance;
            }


            /// <summary>
            /// Registers a configured instance to use as the argument to the parent's
            /// constructor
            /// </summary>
            /// <param name="child"></param>
            /// <returns></returns>
            public ConfiguredInstance Is(Instance child)
            {
                _instance.SetChild(_propertyName, child);
                return _instance;
            }
        }

        #endregion

        #region Nested type: PropertyExpression

        /// <summary>
        /// Defines the value of a primitive argument to a constructur argument
        /// </summary>
        public class PropertyExpression
        {
            private readonly ConfiguredInstance _instance;
            private readonly string _propertyName;

            public PropertyExpression(ConfiguredInstance instance, string propertyName)
            {
                _instance = instance;
                _propertyName = propertyName;
            }

            /// <summary>
            /// Sets the value of the constructor argument
            /// </summary>
            /// <param name="propertyValue"></param>
            /// <returns></returns>
            public ConfiguredInstance EqualTo(object propertyValue)
            {
                _instance.SetProperty(_propertyName, propertyValue.ToString());
                return _instance;
            }

            /// <summary>
            /// Sets the value of the constructor argument to the key/value in the 
            /// AppSettings
            /// </summary>
            /// <param name="appSettingKey"></param>
            /// <returns></returns>
            public ConfiguredInstance EqualToAppSetting(string appSettingKey)
            {
                string propertyValue = ConfigurationManager.AppSettings[appSettingKey];
                _instance.SetProperty(_propertyName, propertyValue);
                return _instance;
            }
        }

        #endregion

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