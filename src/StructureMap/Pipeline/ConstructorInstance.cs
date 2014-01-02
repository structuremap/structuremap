using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using StructureMap.Building;
using StructureMap.Construction;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace StructureMap.Pipeline
{
    public class ConstructorInstance : Instance, IConfiguredInstance, IStructuredInstance
    {
        private readonly Type _pluggedType;
        private readonly DependencyCollection _dependencies = new DependencyCollection();

        [Obsolete("Going to eliminate the need for this")]
        private readonly Plugin _plugin;

        public ConstructorInstance(Type pluggedType)
            : this(new Plugin(pluggedType))
        {
        }

        public ConstructorInstance(Plugin plugin)
        {
            _plugin = plugin;
            _pluggedType = plugin.PluggedType;

            _pluggedType.GetCustomAttributes(typeof (InstanceAttribute), false).OfType<InstanceAttribute>()
                        .Each(x => x.Alter(this));
        }

        public ConstructorInstance(Type pluggedType, string name)
            : this(pluggedType)
        {
            Name = name;
        }

        public Plugin Plugin
        {
            get { return _plugin; }
        }

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

        // Seems to exist only for testing the Xml configuration
        [Obsolete("Get rid of this when we can expose the Dependencies")]
        public string GetProperty(string propertyName)
        {
            var value = _dependencies.FindByTypeOrName(null, propertyName);

            if (value is ObjectInstance)
            {
                return value.As<ObjectInstance>().Object.ToString();
            }

            return value == null ? null : value.ToString();
        }

        [Obsolete("Expose the dependencies instead")]
        public object Get(string propertyName, Type pluginType, BuildSession session)
        {
            var value = _dependencies.FindByTypeOrName(pluginType, propertyName);

            if (value == null)
            {
                if (pluginType.IsSimple() || pluginType == typeof(string))
                {
                    throw new StructureMapException(205, propertyName, Name);
                }

                return new DefaultInstance().Build(pluginType, session);
            }

            return value is Instance ? value.As<Instance>().Build(pluginType, session) : value;
        }

        [Obsolete("Seems to only be used in testing and for the Obsolete Argument")]
        public T Get<T>(string propertyName, BuildSession session)
        {
            object o = Get(propertyName, typeof (T), session);
            if (o == null) return default(T);

            return (T) o;
        }

        public Type PluggedType
        {
            get { return _pluggedType; }
        }

        public bool HasProperty(string propertyName, BuildSession session)
        {
            return _dependencies.Has(propertyName);
        }

        [Obsolete("Just expose DependencyCollection")]
        Instance IStructuredInstance.GetChild(string name)
        {
            return _dependencies.FindByTypeOrName(null, name) as Instance;
        }

        [Obsolete("Just expose DependencyCollection")]
        Instance[] IStructuredInstance.GetChildArray(string name)
        {
            return _dependencies.FindByTypeOrName(null, name)
                .As<EnumerableInstance>().Children.ToArray();
        }

        void IStructuredInstance.RemoveKey(string name)
        {
            _dependencies.RemoveByName(name);
        }

        protected override bool canBePartOfPluginFamily(PluginFamily family)
        {
            return _pluggedType.CanBeCastTo(family.PluginType);
        }

        public ConstructorInstance Override(ExplicitArguments arguments)
        {
            var instance = new ConstructorInstance(_pluggedType);
            _dependencies.CopyTo(instance._dependencies);

            arguments.Configure(instance);

            return instance;
        }

        protected override sealed string getDescription()
        {
            return "Configured Instance of " + _pluggedType.AssemblyQualifiedName;
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            return new LifecycleDependencySource(pluginType, this);
        }

        protected override sealed Type getConcreteType(Type pluginType)
        {
            return _pluggedType;
        }

        internal void SetChild(string name, Instance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance", "Instance for {0} was null".ToFormat(name));
            }

            _dependencies.Add(name, instance);
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
                                                          .ToFormat(_pluggedType.AssemblyQualifiedName, name));
            }
            return dependencyType;
        }

        internal void SetCollection(string name, IEnumerable<Instance> children)
        {
            Type dependencyType = getDependencyType(name);
            var instance = new EnumerableInstance(dependencyType, children);
            SetChild(name, instance);
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
            // TODO -- make this Lazy for crying out loud
            var plan = StructureMap.Building.ConcreteType.BuildPlan(_pluggedType, null, _dependencies, Policies);

            return Build(pluginType, session, plan);
        }

        public object Build(Type pluginType, BuildSession session, IBuildPlan builder)
        {
            if (builder == null)
            {
                throw new StructureMapException(
                    201, _pluggedType.FullName, Name, pluginType);
            }


            try
            {
                return builder.Build(session);
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
            if (!_pluggedType.IsOpenGeneric())
                return null;

            Type closedType;
            try
            {
                closedType = _pluggedType.MakeGenericType(types);
            }
            catch
            {
                return null;
            }

            var closedInstance = new ConstructorInstance(closedType);

            _dependencies.Each(arg => {
                closedInstance._dependencies.Add(arg.CloseType(types));
            });

            return closedInstance;
        }

        public override string ToString()
        {
            return "'{0}' -> {1}".ToFormat(Name, _pluggedType.FullName);
        }
    }


    public abstract class ConstructorInstance<TThis> : ConstructorInstance where TThis : ConstructorInstance
    {
        public ConstructorInstance(Type pluggedType) : base(pluggedType)
        {
        }

        public ConstructorInstance(Plugin plugin) : base(plugin)
        {
        }

        public ConstructorInstance(Type pluggedType, string name) : base(pluggedType, name)
        {
        }

        /// <summary>
        ///     Inline definition of a constructor dependency.  Select the constructor argument by type.  Do not
        ///     use this method if there is more than one constructor arguments of the same type
        /// </summary>
        /// <typeparam name="TCtorType"></typeparam>
        /// <returns></returns>
        public DependencyExpression<TThis, TCtorType> Ctor<TCtorType>()
        {
            string constructorArg = getArgumentNameForType<TCtorType>();
            return Ctor<TCtorType>(constructorArg);
        }

        private string getArgumentNameForType<TCtorType>()
        {
            return Plugin.FindArgumentNameForType<TCtorType>();
        }

        /// <summary>
        ///     Inline definition of a constructor dependency.  Select the constructor argument by type and constructor name.
        ///     Use this method if there is more than one constructor arguments of the same type
        /// </summary>
        /// <typeparam name="TCtorType"></typeparam>
        /// <param name="constructorArg"></param>
        /// <returns></returns>
        public DependencyExpression<TThis, TCtorType> Ctor<TCtorType>(string constructorArg)
        {
            return new DependencyExpression<TThis, TCtorType>(thisObject(), constructorArg);
        }

        protected abstract TThis thisObject();

        /// <summary>
        ///     Inline definition of a setter dependency.  Only use this method if there
        ///     is only a single property of the TSetterType
        /// </summary>
        /// <typeparam name="TSetterType"></typeparam>
        /// <returns></returns>
        public DependencyExpression<TThis, TSetterType> Setter<TSetterType>()
        {
            return Ctor<TSetterType>();
        }

        /// <summary>
        ///     Inline definition of a setter dependency.  Only use this method if there
        ///     is only a single property of the TSetterType
        /// </summary>
        /// <typeparam name="TSetterType"></typeparam>
        /// <param name="setterName">The name of the property</param>
        /// <returns></returns>
        public DependencyExpression<TThis, TSetterType> Setter<TSetterType>(string setterName)
        {
            return Ctor<TSetterType>(setterName);
        }

        /// <summary>
        ///     Inline definition of a dependency on an Array of the CHILD type.  I.e. CHILD[].
        ///     This method can be used for either constructor arguments or setter properties
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <returns></returns>
        public ArrayDefinitionExpression<TThis, TChild> EnumerableOf<TChild>()
        {
            if (typeof (TChild).IsArray)
            {
                throw new ApplicationException("Please specify the element type in the call to TheArrayOf");
            }

            string propertyName = Plugin.FindArgumentNameForEnumerableOf(typeof (TChild));

            if (propertyName.IsEmpty())
            {
                throw new StructureMapException(302, typeof (TChild).FullName, ConcreteType.FullName);
            }
            return new ArrayDefinitionExpression<TThis, TChild>(thisObject(), propertyName);
        }

        /// <summary>
        ///     Inline definition of a dependency on an Array of the CHILD type and the specified setter property or constructor argument name.  I.e. CHILD[].
        ///     This method can be used for either constructor arguments or setter properties
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="ctorOrPropertyName"></param>
        /// <returns></returns>
        public ArrayDefinitionExpression<TThis, TChild> EnumerableOf<TChild>(string ctorOrPropertyName)
        {
            if (ctorOrPropertyName.IsEmpty())
            {
                throw new StructureMapException(302, typeof (TChild).FullName, ConcreteType.FullName);
            }
            return new ArrayDefinitionExpression<TThis, TChild>(thisObject(), ctorOrPropertyName);
        }
    }
}