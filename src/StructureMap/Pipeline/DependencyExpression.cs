using System;
using System.Configuration;
using StructureMap.Configuration.DSL.Expressions;

namespace StructureMap.Pipeline
{
    /// <summary>
    /// Expression Builder that helps to define child dependencies inline 
    /// </summary>
    /// <typeparam name="TChild"></typeparam>
    /// <typeparam name="TInstance"></typeparam>
    public class DependencyExpression<TInstance, TChild> where TInstance : ConstructorInstance
    {
        private readonly TInstance _instance;
        private readonly string _propertyName;

        internal DependencyExpression(TInstance instance, string propertyName)
        {
            _instance = instance;
            _propertyName = propertyName;
        }

        /// <summary>
        /// Sets the value of the constructor argument to the key/value in the 
        /// AppSettings
        /// </summary>
        /// <param name="appSettingKey">The key in appSettings for the value to use.</param>
        /// <returns></returns>
        public TInstance EqualToAppSetting(string appSettingKey)
        {
            return EqualToAppSetting(appSettingKey, null);
        }

        /// <summary>
        /// Sets the value of the constructor argument to the key/value in the 
        /// AppSettings when it exists. Otherwise uses the provided default value.
        /// </summary>
        /// <param name="appSettingKey">The key in appSettings for the value to use.</param>
        /// <param name="defaultValue">The value to use if an entry for <paramref name="appSettingKey"/> does not exist in the appSettings section.</param>
        /// <returns></returns>
        public TInstance EqualToAppSetting(string appSettingKey, string defaultValue)
        {
            string propertyValue = ConfigurationManager.AppSettings[appSettingKey];
            if (propertyValue == null) propertyValue = defaultValue;
            _instance.Dependencies.Add(_propertyName, propertyValue);
            return _instance;
        }

        /// <summary>
        /// Nested Closure to define a child dependency inline
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TInstance Is(Action<IInstanceExpression<TChild>> action)
        {
            var expression = new InstanceExpression<TChild>(i => _instance.Dependencies.Add(_propertyName, i));
            action(expression);

            return _instance;
        }

        public TInstance Is(Func<IContext, TChild> func)
        {
            var child = new LambdaInstance<TChild>(func);
            return Is(child);
        }

        /// <summary>
        /// Shortcut to set an inline dependency to an Instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public TInstance Is(Instance instance)
        {
            _instance.Dependencies.Add(_propertyName, instance);
            return _instance;
        }

        /// <summary>
        /// Shortcut to set an inline dependency to a designated object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TInstance Is(TChild value)
        {
            _instance.Dependencies.Add(_propertyName, value);
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
            return Is(new SmartInstance<TConcreteType>());
        }


        /// <summary>
        /// Shortcut method to define a child dependency inline and configure
        /// the child dependency
        /// </summary>
        /// <typeparam name="TConcreteType"></typeparam>
        /// <returns></returns>
        public TInstance Is<TConcreteType>(Action<SmartInstance<TConcreteType>> configure) where TConcreteType : TChild
        {
            var instance = new SmartInstance<TConcreteType>();
            configure(instance);
            return Is(instance);
        }

        public TInstance Named(string name)
        {
            return Is(c => c.GetInstance<TChild>(name));
        }
    }
}