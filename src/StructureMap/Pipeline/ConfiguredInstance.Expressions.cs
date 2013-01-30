using System;
using StructureMap.Configuration.DSL;
using StructureMap.Interceptors;

namespace StructureMap.Pipeline
{
    public partial class ConfiguredInstance
    {
        public ConfiguredInstance Named(string instanceKey)
        {
            Name = instanceKey;
            return this;
        }

        /// <summary>
        /// Register an Action to perform on the object created by this Instance
        /// before it is returned to the caller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ConfiguredInstance OnCreation<T>(Action<T> handler)
        {
            var interceptor = new StartupInterceptor<T>((c, o) => handler(o));
            Interceptor = interceptor;

            return this;
        }

        /// <summary>
        /// Register an Action to perform on the object created by this Instance
        /// before it is returned to the caller
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ConfiguredInstance OnCreation<T>(Action<IContext, T> handler)
        {
            var interceptor = new StartupInterceptor<T>(handler);
            Interceptor = interceptor;

            return this;
        }

        /// <summary>
        /// Register a Func to potentially enrich or substitute for the object
        /// created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ConfiguredInstance EnrichWith<T>(EnrichmentHandler<T> handler)
        {
            var interceptor = new EnrichmentInterceptor<T>((c, o) => handler(o));
            Interceptor = interceptor;

            return this;
        }

        /// <summary>
        /// Register a Func to potentially enrich or substitute for the object
        /// created by this Instance before it is returned to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public ConfiguredInstance EnrichWith<T>(ContextEnrichmentHandler<T> handler)
        {
            var interceptor = new EnrichmentInterceptor<T>(handler);
            Interceptor = interceptor;

            return this;
        }

        /// <summary>
        /// Inline definition of a dependency array like IService[] or IHandler[]
        /// </summary>
        /// <typeparam name="TPluginType"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public ChildArrayExpression ChildArray<TPluginType>(string propertyName)
        {
            var expression =
                new ChildArrayExpression(this, propertyName);

            return expression;
        }

        /// <summary>
        /// Inline definition of a dependency array like IService[] or IHandler[]
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public ChildArrayExpression ChildArray(string propertyName)
        {
            return new ChildArrayExpression(this, propertyName);
        }

        public ChildArrayExpression ChildArray(Type pluginType)
        {
            string propertyName = findPropertyName(pluginType);
            return ChildArray(propertyName);
        }

        /// <summary>
        /// Inline definition of a dependency array like IService[] or IHandler[]
        /// </summary>
        /// <typeparam name="TPluginType"></typeparam>
        /// <returns></returns>
        public ChildArrayExpression ChildArray<TPluginType>()
        {
            return ChildArray(typeof (TPluginType));
        }

        #region Nested type: ChildArrayExpression

        public class ChildArrayExpression
        {
            private readonly ConfiguredInstance _instance;
            private readonly string _propertyName;

            public ChildArrayExpression(ConfiguredInstance instance, string propertyName)
            {
                _instance = instance;
                _propertyName = propertyName;
            }

            /// <summary>
            /// Configures an array of Instance's for the array dependency
            /// </summary>
            /// <param name="instances"></param>
            /// <returns></returns>
            public ConfiguredInstance Contains(params Instance[] instances)
            {
                _instance.SetCollection(_propertyName, instances);

                return _instance;
            }
        }

        #endregion

    }
}