using System;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL.Expressions
{
    /// <summary>
    /// Expression class to help define a runtime Profile
    /// </summary>
    public class ProfileExpression
    {
        private readonly string _profileName;
        private readonly Registry _registry;

        public ProfileExpression(string profileName, Registry registry)
        {
            _profileName = profileName;
            _registry = registry;
        }


        /// <summary>
        /// Starts the definition of the default instance for the containing Profile.  This is
        /// still valid, but Type() is recommended
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public InstanceDefaultExpression<T> For<T>()
        {
            return new InstanceDefaultExpression<T>(this);
        }

        /// <summary>
        /// Use statement to define the Profile defaults for a Generic type
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public GenericDefaultExpression For(Type pluginType)
        {
            return new GenericDefaultExpression(this, pluginType);
        }

        #region Nested type: GenericDefaultExpression

        /// <summary>
        /// Expression Builder inside of a Profile creation for
        /// open generic types
        /// </summary>
        public class GenericDefaultExpression
        {
            private readonly ProfileExpression _parent;
            private readonly Type _pluginType;
            private readonly Registry _registry;

            internal GenericDefaultExpression(ProfileExpression parent, Type pluginType)
            {
                _parent = parent;
                _registry = parent._registry;
                _pluginType = pluginType;
            }

            /// <summary>
            /// Use this concreteType for the Instance of this Profile for the PluginType
            /// </summary>
            /// <param name="concreteType"></param>
            /// <returns></returns>
            public ConfiguredInstance Use(Type concreteType)
            {
                var instance = new ConfiguredInstance(concreteType);
                Use(instance);

                return instance;
            }

            /// <summary>
            /// Use this Instance for the Profile Instance of this Plugin Type
            /// </summary>
            /// <param name="instance"></param>
            /// <returns></returns>
            public void Use(Instance instance)
            {
                _registry.addExpression(graph => graph.SetDefault(_parent._profileName, _pluginType, instance));
            }

            /// <summary>
            /// Use the named Instance as the Profile Instance for this PluginType
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public ReferencedInstance Use(string name)
            {
                var instance = new ReferencedInstance(name);
                Use(instance);

                return instance;
            }

            /// <summary>
            /// For this type and profile, build the object with this Lambda
            /// </summary>
            /// <param name="func"></param>
            /// <returns></returns>
            public LambdaInstance<object> Use(Func<IContext, object> func)
            {
                var instance = new LambdaInstance<object>(func);
                Use(instance);

                return instance;
            }
        }

        #endregion

        #region Nested type: InstanceDefaultExpression

        /// <summary>
        /// Expression Builder within defining a Profile
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class InstanceDefaultExpression<T>
        {
            private readonly string _profileName;
            private readonly Registry _registry;

            public InstanceDefaultExpression(ProfileExpression parent)
            {
                _registry = parent._registry;
                _profileName = parent._profileName;
            }

            /// <summary>
            /// Use a named, preconfigured instance as the default instance for this profile 
            /// </summary>
            /// <param name="instanceKey"></param>
            /// <returns></returns>
            public void Use(string instanceKey)
            {
                _registry.addExpression(
                    graph => graph.SetDefault(_profileName, typeof(T), new ReferencedInstance(instanceKey)));
            }

            /// <summary>
            /// Define the default instance of the PluginType for the containing Profile
            /// </summary>
            /// <param name="instance"></param>
            /// <returns></returns>
            public void Use(Instance instance)
            {
                instance.Name = "Default Instance for Profile " + _profileName;

                _registry.addExpression(graph => graph.SetDefault(_profileName, typeof (T), instance));
            }

            /// <summary>
            /// For this Profile, use an Instance with this Func
            /// </summary>
            /// <param name="func"></param>
            /// <returns></returns>
            public LambdaInstance<T> Use(Func<T> func)
            {
                var instance = new LambdaInstance<T>(func);
                Use(instance);

                return instance;
            }

            /// <summary>
            /// For this Profile, use an Instance with this Func
            /// </summary>
            /// <param name="func"></param>
            /// <returns></returns>
            public LambdaInstance<T> Use(Func<IContext, T> func)
            {
                var instance = new LambdaInstance<T>(func);
                Use(instance);

                return instance;
            }

            /// <summary>
            /// For this Profile, use this object
            /// </summary>
            /// <param name="t"></param>
            /// <returns></returns>
            public ObjectInstance Use(T t)
            {
                var instance = new ObjectInstance(t);
                Use(instance);

                return instance;
            }

            /// <summary>
            /// Access to the uncommon types of Instance
            /// </summary>
            /// <param name="configure"></param>
            public void UseSpecial(Action<IInstanceExpression<T>> configure)
            {
                var expression = new InstanceExpression<T>(i => Use(i));
                configure(expression);
            }

            /// <summary>
            /// For this profile, use this concrete type
            /// </summary>
            /// <typeparam name="TConcreteType"></typeparam>
            /// <returns></returns>
            public SmartInstance<TConcreteType> Use<TConcreteType>()
            {
                var instance = new SmartInstance<TConcreteType>();
                Use(instance);

                return instance;
            }
        }

        #endregion
    }
}