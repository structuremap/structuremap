using System;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.Building;
using StructureMap.Building.Interception;

namespace StructureMap.Pipeline
{
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
            var finder = new ConstructorFinderVisitor();
            finder.Visit(constructor);

            _inner.Constructor = finder.Constructor;

            return this;
        }

        protected override SmartInstance<T, TPluginType> thisInstance
        {
            get { return this; }
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

        public override string Description
        {
            get { return _inner.Description; }
        }

        public override Type ReturnedType
        {
            get { return PluggedType; }
        }

        public Type PluggedType
        {
            get
            {
                return typeof (T);
            }
        }

        public DependencyCollection Dependencies
        {
            get
            {
                return _inner.Dependencies;
            }
        }

        public ConstructorInstance Override(ExplicitArguments arguments)
        {
            return _inner.Override(arguments);
        }

        public ConstructorInfo Constructor
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
    }

}