using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using StructureMap.Building.Interception;

namespace StructureMap.Pipeline
{
    /// <summary>
    ///     Instance that builds objects with by calling constructor functions and using setter properties
    /// </summary>
    /// <typeparam name="T">The concrete type constructed by SmartInstance</typeparam>
    public class SmartInstance<T> : ConstructorInstance<SmartInstance<T>>
    {
        private readonly List<Action<T>> _actions = new List<Action<T>>();

        public SmartInstance()
            : base(typeof (T))
        {
        }

        /// <summary>
        ///     Sets the name of this Instance
        /// </summary>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public SmartInstance<T> Named(string instanceKey)
        {
            Name = instanceKey;
            return this;
        }

        /// <summary>
        ///     Register an Action to perform on the object created by this Instance
        ///     before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public SmartInstance<T> OnCreation(Expression<Action<T>> handler)
        {
            AddInterceptor(new ActivatorInterceptor<T>(handler));

            return this;
        }

        /// <summary>
        ///     Register an Action to perform on the object created by this Instance
        ///     before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="description">Descriptive text for diagnostic purposes</param>
        /// <returns></returns>
        public SmartInstance<T> OnCreation(string description, Action<T> handler)
        {
            AddInterceptor(InterceptorFactory.ForAction(description, handler));

            return this;
        }

        protected override SmartInstance<T> thisObject()
        {
            return this;
        }

        /// <summary>
        ///     Register an Action to perform on the object created by this Instance
        ///     before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public SmartInstance<T> OnCreation(Expression<Action<IBuildSession, T>> handler)
        {
            AddInterceptor(new ActivatorInterceptor<T>(handler));

            return this;
        }

        /// <summary>
        ///     Register an Action to perform on the object created by this Instance
        ///     before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="description">Descriptive text for diagnostic purposes</param>
        /// <returns></returns>
        public SmartInstance<T> OnCreation(string description, Action<IBuildSession, T> handler)
        {
            AddInterceptor(InterceptorFactory.ForAction(description, handler));

            return this;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public SmartInstance<T> EnrichWith(Expression<Func<T, T>> handler)
        {
            AddInterceptor(new FuncInterceptor<T>(handler));

            return this;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="description">Descriptive text for diagnostic purposes</param>
        /// <returns></returns>
        public SmartInstance<T> EnrichWith(string description, Func<T, T> handler)
        {
            AddInterceptor(InterceptorFactory.ForFunc(description, handler));

            return this;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public SmartInstance<T> EnrichWith<TPluginType>(Expression<Func<TPluginType, TPluginType>> handler)
        {
            AddInterceptor(new FuncInterceptor<TPluginType>(handler));

            return this;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="description">Descriptive text for diagnostic purposes</param>
        /// <returns></returns>
        public SmartInstance<T> EnrichWith<TPluginType>(string description, Func<TPluginType, TPluginType> handler)
        {
            AddInterceptor(InterceptorFactory.ForFunc(description, handler));

            return this;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public SmartInstance<T> EnrichWith(Expression<Func<IBuildSession, T, T>> handler)
        {
            AddInterceptor(new FuncInterceptor<T>(handler));

            return this;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="description">Descriptive text for diagnostic purposes</param>
        /// <returns></returns>
        public SmartInstance<T> EnrichWith(string description, Func<IBuildSession, T, T> handler)
        {
            AddInterceptor(InterceptorFactory.ForFunc(description, handler));

            return this;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public SmartInstance<T> EnrichWith<TPluginType>(Expression<Func<IBuildSession, TPluginType, TPluginType>> handler)
        {
            AddInterceptor(new FuncInterceptor<TPluginType>(handler));

            return this;
        }


        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="description">Descriptive text for diagnostic purposes</param>
        /// <returns></returns>
        public SmartInstance<T> EnrichWith<TPluginType>(string description, Func<IBuildSession, TPluginType, TPluginType> handler)
        {
            AddInterceptor(InterceptorFactory.ForFunc(description, handler));

            return this;
        }

        /// <summary>
        ///     Register an <see cref="IInterceptor">IInterceptor</see> with this Instance
        /// </summary>
        /// <param name="interceptor"></param>
        /// <returns></returns>
        public SmartInstance<T> InterceptWith(IInterceptor interceptor)
        {
            AddInterceptor(interceptor);
            return this;
        }

        protected override object build(Type pluginType, IBuildSession session)
        {
            var builtTarget = (T) base.build(pluginType, session);
            foreach (var action in _actions)
            {
                action(builtTarget);
            }

            return builtTarget;
        }

        /// <summary>
        ///     Set simple setter properties
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public SmartInstance<T> SetProperty(Action<T> action)
        {
            _actions.Add(action);
            return this;
        }


        /// <summary>
        ///     Inline definition of a setter dependency.  The property name is specified with an Expression
        /// </summary>
        /// <typeparam name="TSettertype"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DependencyExpression<SmartInstance<T>, TSettertype> Setter<TSettertype>(
            Expression<Func<T, TSettertype>> expression)
        {
            var propertyName = ReflectionHelper.GetProperty(expression).Name;
            return new DependencyExpression<SmartInstance<T>, TSettertype>(this, propertyName);
        }
    }
}