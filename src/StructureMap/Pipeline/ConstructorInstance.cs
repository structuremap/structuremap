using System;
using System.IO;
using System.Linq;
using System.Reflection;
using StructureMap.Building;
using StructureMap.Diagnostics;
using StructureMap.TypeRules;


namespace StructureMap.Pipeline
{
    public class ConstructorInstance : ConstructorInstance<ConstructorInstance>
    {
        public ConstructorInstance(Type concreteType) : base(concreteType)
        {
        }

        public ConstructorInstance(Type pluggedType, string name) : base(pluggedType, name)
        {
        }

        protected override ConstructorInstance thisInstance => this;

        protected override ConstructorInstance thisObject()
        {
            return this;
        }
    }


    public abstract class ConstructorInstance<TThis> : ExpressedInstance<TThis>, IConfiguredInstance, IOverridableInstance
        where TThis : ConstructorInstance<TThis>
    {
        public ConstructorInstance(Type concreteType)
        {
            if (!concreteType.IsConcrete())
            {
                throw new ArgumentOutOfRangeException("concreteType", concreteType,"Only concrete types can be built by ConstructorInstance.");
            }

            if (!concreteType.GetPublicAndInternalConstructors().Any())
            {
                throw new ArgumentOutOfRangeException(
                    "{0} must have at least one public constructor to be plugged in by StructureMap".ToFormat(
                        concreteType.GetFullName()));
            }

            if (concreteType.GetPublicAndInternalConstructors().Count() == 1)
            {
                Constructor = concreteType.GetPublicAndInternalConstructors().Single();
            }

            PluggedType = concreteType;

            PluggedType.GetTypeInfo().ForAttribute<StructureMapAttribute>(x => x.Alter(this));
        }

        public ConstructorInstance(Type pluggedType, string name)
            : this(pluggedType)
        {
            Name = name;
        }

        public override Instance ToNamedClone(string name)
        {
            return new ConfiguredInstance(PluggedType, name, Dependencies, Interceptors, Constructor);
        }

        /// <summary>
        /// Explicitly select a constructor
        /// </summary>
        public ConstructorInfo Constructor { get; set; }


        public Type PluggedType { get; }

        public override IDependencySource ToBuilder(Type pluginType, Policies policies)
        {
            var plan = ConcreteType.BuildSource(PluggedType, Constructor, Dependencies, policies);
            if (!plan.IsValid())
            {
                var message = "Unable to create a build plan for concrete type " + PluggedType.GetFullName();

                var visualizer = new BuildPlanVisualizer(null, levels: 0);
                plan.AcceptVisitor(visualizer);
                var writer = new StringWriter();
                visualizer.Write(writer);

                throw new StructureMapBuildPlanException(message)
                {
                    Context = writer.ToString()
                };
            }

            return plan;
        }

        public Instance Override(ExplicitArguments arguments)
        {
            if (arguments.OnlyNeedsDefaults() && !Dependencies.HasAny())
            {
                return this;
            }

            var instance = new ConstructorInstance(PluggedType) {Name = Name};

            Dependencies.CopyTo(instance.Dependencies);

            arguments.Configure(instance);

            return instance;
        }

        public override string Description => HasExplicitName()
            ? "{0} ('{1}')".ToFormat(PluggedType.GetFullName(), Name)
            : PluggedType.GetFullName();

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            return new LifecycleDependencySource(pluginType, this);
        }

        public override Type ReturnedType => PluggedType;


        public static ConstructorInstance For<T>()
        {
            return new ConstructorInstance(typeof (T));
        }

        public override Instance CloseType(Type[] types)
        {
            if (!PluggedType.IsOpenGeneric())
                return null;

            Type closedType;
            try
            {
                closedType = PluggedType.MakeGenericType(types);
            }
            catch
            {
                return null;
            }

            var closedInstance = new ConstructorInstance(closedType);

            Dependencies.Each(arg => closedInstance.Dependencies.Add(arg.CloseType(types)));

            return closedInstance;
        }

        public DependencyCollection Dependencies { get; } = new DependencyCollection();

        public override string ToString()
        {
            return "'{0}' -> {1}".ToFormat(Name, PluggedType.FullName);
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
                throw new ArgumentException("Please specify the element type in the call to TheArrayOf");
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