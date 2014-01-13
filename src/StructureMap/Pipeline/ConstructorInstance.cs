using System;
using System.Linq;
using StructureMap.Building;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace StructureMap.Pipeline
{
    public class ConstructorInstance : Instance, IConfiguredInstance
    {
        private readonly Type _pluggedType;
        private readonly DependencyCollection _dependencies = new DependencyCollection();

        public ConstructorInstance(Type concreteType)
        {
            _pluggedType = concreteType;

            _pluggedType.GetCustomAttributes(typeof (InstanceAttribute), false).OfType<InstanceAttribute>()
                .Each(x => x.Alter(this));
        }

        public ConstructorInstance(Type pluggedType, string name)
            : this(pluggedType)
        {
            Name = name;
        }

        public Type PluggedType
        {
            get { return _pluggedType; }
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


        protected override object build(Type pluginType, IBuildSession session)
        {
            // TODO -- make this Lazy for crying out loud
            var plan = StructureMap.Building.ConcreteType.BuildPlan(_pluggedType, null, _dependencies, Policies);
            

            return Build(pluginType, session, plan);
        }

        public object Build(Type pluginType, IBuildSession session, IBuildPlan builder)
        {
            try
            {
                return builder.Build(session);
            }
            catch (StructureMapException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new StructureMapBuildException("Failed while building '{0}'".ToFormat(Description), ex);
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

            _dependencies.Each(arg => { closedInstance._dependencies.Add(arg.CloseType(types)); });

            return closedInstance;
        }

        public DependencyCollection Dependencies
        {
            get { return _dependencies; }
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
            return Ctor<TCtorType>(null);
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
        /// <typeparam name="TElement"></typeparam>
        /// <returns></returns>
        public ArrayDefinitionExpression<TThis, TElement> EnumerableOf<TElement>()
        {
            if (typeof (TElement).IsArray)
            {
                throw new ApplicationException("Please specify the element type in the call to TheArrayOf");
            }

            return new ArrayDefinitionExpression<TThis, TElement>(thisObject(), null);
        }

        /// <summary>
        ///     Inline definition of a dependency on an Array of the CHILD type and the specified setter property or constructor argument name.  I.e. CHILD[].
        ///     This method can be used for either constructor arguments or setter properties
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="ctorOrPropertyName"></param>
        /// <returns></returns>
        public ArrayDefinitionExpression<TThis, TElement> EnumerableOf<TElement>(string ctorOrPropertyName)
        {
            return new ArrayDefinitionExpression<TThis, TElement>(thisObject(), ctorOrPropertyName);
        }
    }
}