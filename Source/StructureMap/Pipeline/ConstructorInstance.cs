using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using StructureMap.Construction;
using StructureMap.Graph;
using StructureMap.TypeRules;
using StructureMap.Util;

namespace StructureMap.Pipeline
{
    public class ConstructorInstance : Instance, IConfiguredInstance, IStructuredInstance
    {
        private readonly Cache<string, Instance> _dependencies = new Cache<string, Instance>();
        private readonly Plugin _plugin;

        public ConstructorInstance(Type pluggedType)
            : this(PluginCache.GetPlugin(pluggedType))
        {
        }

        public ConstructorInstance(Plugin plugin)
        {
            _plugin = plugin;

            _dependencies.OnMissing = key =>
            {
                if (_plugin.FindArgumentType(key).IsSimple())
                {
                    throw new StructureMapException(205, key, Name);
                }

                return new DefaultInstance();
            };
        }

        public ConstructorInstance(Type pluggedType, string name)
            : this(pluggedType)
        {
            Name = name;
        }

        protected Plugin plugin { get { return _plugin; } }

        void IConfiguredInstance.SetChild(string name, Instance instance)
        {
            SetChild(name, instance);
        }

        public void SetValue(Type type, object value, CannotFindProperty cannotFind)
        {
            string name = _plugin.FindArgumentNameForType(type, cannotFind);
            if (name != null) SetValue(name, value);
        }

        void IConfiguredInstance.SetValue(string name, object value)
        {
            SetValue(name, value);
        }

        void IConfiguredInstance.SetCollection(string name, IEnumerable<Instance> children)
        {
            SetCollection(name, children);
        }

        public string GetProperty(string propertyName)
        {
            return _dependencies[propertyName].As<ObjectInstance>().Object.ToString();
        }

        public object Get(string propertyName, Type pluginType, BuildSession session)
        {
            return _dependencies[propertyName].Build(pluginType, session);
        }

        public T Get<T>(string propertyName, BuildSession session)
        {
            object o = Get(propertyName, typeof (T), session);
            if (o == null) return default(T);

            return (T) o;
        }

        public Type PluggedType { get { return _plugin.PluggedType; } }

        public bool HasProperty(string propertyName, BuildSession session)
        {
            // TODO -- richer behavior
            return _dependencies.Has(propertyName);
        }

        Instance IStructuredInstance.GetChild(string name)
        {
            return _dependencies[name];
        }

        Instance[] IStructuredInstance.GetChildArray(string name)
        {
            return _dependencies[name].As<EnumerableInstance>().Children.ToArray();
        }

        void IStructuredInstance.RemoveKey(string name)
        {
            _dependencies.Remove(name);
        }

        protected override bool canBePartOfPluginFamily(PluginFamily family)
        {
            return _plugin.PluggedType.CanBeCastTo(family.PluginType);
        }

        public ConstructorInstance Override(ExplicitArguments arguments)
        {
            var instance = new ConstructorInstance(_plugin);
            _dependencies.Each((key, i) => instance.SetChild(key, i));

            arguments.Configure(instance);

            return instance;
        }

        protected override sealed string getDescription()
        {
            return "Configured Instance of " + _plugin.PluggedType.AssemblyQualifiedName;
        }

        protected override sealed Type getConcreteType(Type pluginType)
        {
            return _plugin.PluggedType;
        }

        internal void SetChild(string name, Instance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance", "Instance for {0} was null".ToFormat(name));
            }

            _dependencies[name] = instance;
        }

        internal void SetValue(string name, object value)
        {
            Type dependencyType = getDependencyType(name);

            Instance instance = buildInstanceForType(dependencyType, value);
            SetChild(name, instance);
        }

        private Type getDependencyType(string name)
        {
            Type dependencyType = _plugin.FindArgumentType(name);
            if (dependencyType == null)
            {
                throw new ArgumentOutOfRangeException("name",
                                                      "Could not find a constructor parameter or property for {0} named {1}"
                                                          .ToFormat(_plugin.PluggedType.AssemblyQualifiedName, name));
            }
            return dependencyType;
        }

        internal void SetCollection(string name, IEnumerable<Instance> children)
        {
            Type dependencyType = getDependencyType(name);
            var instance = new EnumerableInstance(dependencyType, children);
            SetChild(name, instance);
        }

        protected string findPropertyName<PLUGINTYPE>()
        {
            Type dependencyType = typeof (PLUGINTYPE);

            return findPropertyName(dependencyType);
        }

        protected string findPropertyName(Type dependencyType)
        {
            string propertyName = _plugin.FindArgumentNameForType(dependencyType, CannotFindProperty.ThrowException);

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new StructureMapException(305, dependencyType);
            }

            return propertyName;
        }

        private Instance buildInstanceForType(Type dependencyType, object value)
        {
            if (value == null) return new NullInstance();


            if (dependencyType.IsSimple() || dependencyType.IsNullable() || dependencyType == typeof (Guid) ||
                dependencyType == typeof (DateTime))
            {
                try
                {
                    if (value.GetType() == dependencyType) return new ObjectInstance(value);

                    TypeConverter converter = TypeDescriptor.GetConverter(dependencyType);
                    object convertedValue = converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
                    return new ObjectInstance(convertedValue);
                }
                catch (Exception e)
                {
                    throw new StructureMapException(206, e, Name);
                }
            }


            return new ObjectInstance(value);
        }

        protected override object build(Type pluginType, BuildSession session)
        {
            IInstanceBuilder builder = PluginCache.FindBuilder(_plugin.PluggedType);
            return Build(pluginType, session, builder);
        }

        public object Build(Type pluginType, BuildSession session, IInstanceBuilder builder)
        {
            if (builder == null)
            {
                throw new StructureMapException(
                    201, _plugin.PluggedType.FullName, Name, pluginType);
            }


            try
            {
                var args = new Arguments(this, session);
                return builder.BuildInstance(args);
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

        public static ConstructorInstance For<T>()
        {
            return new ConstructorInstance(typeof (T));
        }

        public override Instance CloseType(Type[] types)
        {
            if(!_plugin.PluggedType.IsOpenGeneric())
                return null;

            Type closedType;
            try {
                closedType = _plugin.PluggedType.MakeGenericType(types);
            }
            catch {
                return null;
            }

            var closedInstance = new ConstructorInstance(closedType);

            _dependencies.Each((key, i) => {
                                   if(i.CopyAsIsWhenClosingInstance) {
                                       closedInstance.SetChild(key, i);
                                   }
                               });

            return closedInstance;
        }

        public override string ToString()
        {
            return "'{0}' -> {1}".ToFormat(Name, _plugin.PluggedType.FullName);
        }
    }
}
