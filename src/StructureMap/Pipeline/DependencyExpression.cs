using System;
using System.Linq.Expressions;
using StructureMap.Configuration.DSL.Expressions;

namespace StructureMap.Pipeline
{

    /// <summary>
    /// Expression Builder that helps to define child dependencies inline 
    /// </summary>
    public class DependencyExpression<TInstance, TChild> where TInstance : IConfiguredInstance
    {
        private readonly TInstance _instance;
        private readonly string _propertyName;

        internal DependencyExpression(TInstance instance, string propertyName)
        {
            _instance = instance;
            _propertyName = propertyName;
        }

        /// <summary>
        /// Nested Closure to define a child dependency inline
        /// </summary>
        public TInstance IsSpecial(Action<IInstanceExpression<TChild>> action)
        {
            var expression =
                new InstanceExpression<TChild>(i => _instance.Dependencies.Add(_propertyName, typeof (TChild), i));
            action(expression);

            return _instance;
        }

        /// <summary>
        /// Inline dependency by simple Lambda expression
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public TInstance Is(Expression<Func<TChild>> func)
        {
            var child = new LambdaInstance<TChild>(func);
            return Is(child);
        }

        /// <summary>
        /// Inline dependency by Lambda Func
        /// </summary>
        /// <param name="description">User friendly description for diagnostics</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public TInstance Is(string description, Func<TChild> func)
        {
            var child = new LambdaInstance<TChild>(description, func);
            return Is(child);
        }

        /// <summary>
        /// Inline dependency by Lambda expression that uses IContext
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public TInstance Is(Expression<Func<IContext, TChild>> func)
        {
            var child = new LambdaInstance<TChild>(func);
            return Is(child);
        }

        /// <summary>
        /// Inline dependency by Lambda Func that uses IContext
        /// </summary>
        /// <param name="description">User friendly description for diagnostics</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public TInstance Is(string description, Func<IContext, TChild> func)
        {
            var child = new LambdaInstance<TChild>(description, func);
            return Is(child);
        }

        /// <summary>
        /// Shortcut to set an inline dependency to an Instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public TInstance Is(Instance instance)
        {
            _instance.Dependencies.Add(_propertyName, typeof (TChild), instance);
            return _instance;
        }

        /// <summary>
        /// Shortcut to set an inline dependency to a designated object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TInstance Is(TChild value)
        {
            _instance.Dependencies.Add(_propertyName, typeof (TChild), value);
            return _instance;
        }

        /// <summary>
        /// Set an Inline dependency to the Default Instance of the Property type
        /// Used mostly to force an optional Setter property to be filled by
        /// StructureMap
        /// </summary>
        /// <returns></returns>
        public TInstance IsTheDefault()
        {
            return Is(new DefaultInstance());
        }

        /// <summary>
        /// Set the inline dependency to the named instance of the property type
        /// Used mostly to force an optional Setter property to be filled by
        /// StructureMap        /// </summary>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public TInstance IsNamedInstance(string instanceKey)
        {
            return Is(new ReferencedInstance(instanceKey));
        }

        /// <summary>
        /// Shortcut method to define a child dependency inline
        /// </summary>
        /// <typeparam name="TConcreteType"></typeparam>
        /// <returns></returns>
        public TInstance Is<TConcreteType>() where TConcreteType : TChild
        {
            return Is(new SmartInstance<TConcreteType, TChild>());
        }


        /// <summary>
        /// Shortcut method to define a child dependency inline and configure
        /// the child dependency
        /// </summary>
        /// <typeparam name="TConcreteType"></typeparam>
        /// <returns></returns>
        public TInstance Is<TConcreteType>(Action<SmartInstance<TConcreteType, TChild>> configure) where TConcreteType : TChild
        {
            var instance = new SmartInstance<TConcreteType, TChild>();
            configure(instance);
            return Is(instance);
        }

        /// <summary>
        /// Use the named Instance of TChild for the inline
        /// dependency here
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TInstance Named(string name)
        {
            return Is(c => c.GetInstance<TChild>(name));
        }
    }
}