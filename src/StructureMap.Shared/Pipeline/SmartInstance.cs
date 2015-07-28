using System;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.Building;
using StructureMap.Building.Interception;

namespace StructureMap.Pipeline
{

    public class SmartInstance<T> : SmartInstance<T, T>
    {
        public SmartInstance(Expression<Func<T>> constructorSelection = null) : base(constructorSelection)
        {
        }
    }

    /// <summary>
    ///     Instance that builds objects with by calling constructor functions and using setter properties
    /// </summary>
    /// <typeparam name="T">The concrete type constructed by SmartInstance</typeparam>
    /// <typeparam name="TPluginType">The "PluginType" that this instance satisfies</typeparam>
    public class SmartInstance<T, TPluginType> : ExpressedInstance<SmartInstance<T, TPluginType>, T, TPluginType>, IConfiguredInstance where T : TPluginType
    {
        private readonly ConstructorInstance _inner = new ConstructorInstance(typeof (T));

        public SmartInstance(Expression<Func<T>> constructorSelection = null)
        {
            if (constructorSelection != null)
            {
                SelectConstructor(constructorSelection);
            }

            
        }

        public SmartInstance<T, TPluginType> SelectConstructor(Expression<Func<T>> constructor)
        {
            var finder = new ConstructorFinderVisitor(typeof(T));
            finder.Visit(constructor);

            _inner.Constructor = finder.Constructor;

            return this;
        }

        protected override SmartInstance<T, TPluginType> thisInstance
        {
            get { return this; }
        }

        public override string Name
        {
            get
            {
                return _inner.Name;
            }
            set
            {
                _inner.Name = value;
            }
        }

        /// <summary>
        ///     Set simple setter properties
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public SmartInstance<T, TPluginType> SetProperty(Action<T> action)
        {
            AddInterceptor(InterceptorFactory.ForAction("Setting property", action));
            return this;
        }


        /// <summary>
        ///     Inline definition of a setter dependency.  The property name is specified with an Expression
        /// </summary>
        /// <typeparam name="TSettertype"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DependencyExpression<SmartInstance<T, TPluginType>, TSettertype> Setter<TSettertype>(
            Expression<Func<T, TSettertype>> expression)
        {
            var propertyName = ReflectionHelper.GetProperty(expression).Name;
            return new DependencyExpression<SmartInstance<T, TPluginType>, TSettertype>(this, propertyName);
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            return _inner.ToDependencySource(pluginType);
        }

        public override IDependencySource ToBuilder(Type pluginType, Policies policies)
        {
            return _inner.ToBuilder(pluginType, policies);
        }

        public override string Description
        {
            get { return _inner.Description; }
        }

        public override Type ReturnedType
        {
            get { return typeof(T); }
        }

        Type IConfiguredInstance.PluggedType
        {
            get
            {
                return typeof (T);
            }
        }

        DependencyCollection IConfiguredInstance.Dependencies
        {
            get
            {
                return _inner.Dependencies;
            }
        }

        ConstructorInstance IConfiguredInstance.Override(ExplicitArguments arguments)
        {
            return _inner.Override(arguments);
        }

        ConstructorInfo IConfiguredInstance.Constructor
        {
            get
            {
                return _inner.Constructor;
            }
            set
            {
                _inner.Constructor = value;
            }
        }




        /// <summary>
        ///     Inline definition of a constructor dependency.  Select the constructor argument by type.  Do not
        ///     use this method if there is more than one constructor arguments of the same type
        /// </summary>
        /// <typeparam name="TCtorType"></typeparam>
        /// <returns></returns>
        public DependencyExpression<SmartInstance<T, TPluginType>, TCtorType> Ctor<TCtorType>()
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
        public DependencyExpression<SmartInstance<T, TPluginType>, TCtorType> Ctor<TCtorType>(string constructorArg)
        {
            return new DependencyExpression<SmartInstance<T, TPluginType>, TCtorType>(this, constructorArg);
        }


        /// <summary>
        ///     Inline definition of a setter dependency.  Only use this method if there
        ///     is only a single property of the TSetterType
        /// </summary>
        /// <typeparam name="TSetterType"></typeparam>
        /// <returns></returns>
        public DependencyExpression<SmartInstance<T, TPluginType>, TSetterType> Setter<TSetterType>()
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
        public DependencyExpression<SmartInstance<T, TPluginType>, TSetterType> Setter<TSetterType>(string setterName)
        {
            return Ctor<TSetterType>(setterName);
        }

        /// <summary>
        ///     Inline definition of a dependency on an Array of the CHILD type.  I.e. CHILD[].
        ///     This method can be used for either constructor arguments or setter properties
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <returns></returns>
        public ArrayDefinitionExpression<SmartInstance<T, TPluginType>, TElement> EnumerableOf<TElement>()
        {
            if (typeof(TElement).IsArray)
            {
                throw new ArgumentException("Please specify the element type in the call to TheArrayOf");
            }

            return new ArrayDefinitionExpression<SmartInstance<T, TPluginType>, TElement>(this, null);
        }

        /// <summary>
        ///     Inline definition of a dependency on an Array of the CHILD type and the specified setter property or constructor argument name.  I.e. CHILD[].
        ///     This method can be used for either constructor arguments or setter properties
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="ctorOrPropertyName"></param>
        /// <returns></returns>
        public ArrayDefinitionExpression<SmartInstance<T, TPluginType>, TElement> EnumerableOf<TElement>(string ctorOrPropertyName)
        {
            return new ArrayDefinitionExpression<SmartInstance<T, TPluginType>, TElement>(this, ctorOrPropertyName);
        }
    }

}