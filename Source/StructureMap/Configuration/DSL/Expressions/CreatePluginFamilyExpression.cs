using System;
using System.Collections.Generic;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL.Expressions
{
    public delegate void AlterPluginFamilyDelegate(PluginFamily family);

    /// <summary>
    /// Represents the parameters for creating instances of a given Type
    /// </summary>
    public class CreatePluginFamilyExpression<PLUGINTYPE> : IExpression
    {
        private readonly List<AlterPluginFamilyDelegate> _alterations = new List<AlterPluginFamilyDelegate>();
        private readonly List<IExpression> _children = new List<IExpression>();
        private readonly Type _pluginType;
        private readonly InstanceScope _scope = InstanceScope.PerRequest;

        public CreatePluginFamilyExpression()
        {
            _pluginType = typeof (PLUGINTYPE);
        }

        #region IExpression Members

        void IExpression.Configure(PluginGraph graph)
        {
            PluginFamily family = graph.LocateOrCreateFamilyForType(_pluginType);
            InterceptorChainBuilder builder = new InterceptorChainBuilder();
            family.InterceptionChain = builder.Build(_scope);

            foreach (IExpression child in _children)
            {
                child.Configure(graph);
            }

            foreach (AlterPluginFamilyDelegate alteration in _alterations)
            {
                alteration(family);
            }

            graph.PluginFamilies.Add(family);

            AssemblyGraph assembly = new AssemblyGraph(_pluginType.Assembly);
            graph.Assemblies.Add(assembly);
        }

        #endregion

        /// <summary>
        /// Sets the default instance of a Type to the definition represented by builder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> TheDefaultIs(Instance instance)
        {
            _alterations.Add(delegate(PluginFamily family)
                                 {
                                     family.AddInstance(instance);
                                     family.DefaultInstanceKey = instance.Name;
                                 });

            return this;
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> AddInstance(Instance instance)
        {
            // TODO:  Validate pluggability
            _alterations.Add(delegate(PluginFamily family)
                                 {
                                     family.AddInstance(instance);
                                 });

            return this;
        }

        /// <summary>
        /// Convenience method that sets the default concrete type of the PluginType.  Type T
        /// can only accept types that do not have any primitive constructor arguments.
        /// StructureMap has to know how to construct all of the constructor argument types.
        /// </summary>
        /// <typeparam name="CONCRETETYPE"></typeparam>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> TheDefaultIsConcreteType<CONCRETETYPE>()
            where CONCRETETYPE : PLUGINTYPE
        {
            ExpressionValidator.ValidatePluggabilityOf(typeof (CONCRETETYPE)).IntoPluginType(_pluginType);

            _alterations.Add(delegate(PluginFamily family)
                                 {
                                     Plugin plugin = family.Plugins.FindOrCreate(typeof (CONCRETETYPE), true);
                                     family.DefaultInstanceKey = plugin.ConcreteKey;
                                 });

            return this;
        }

        /// <summary>
        /// Sets the object creation of the instances of the PluginType.  For example:  PerRequest,
        /// Singleton, ThreadLocal, HttpContext, or Hybrid
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> CacheBy(InstanceScope scope)
        {
            _alterations.Add(delegate(PluginFamily family)
                                 {
                                     InterceptorChainBuilder builder = new InterceptorChainBuilder();
                                     family.InterceptionChain = builder.Build(scope);
                                 });

            return this;
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as a Singleton
        /// </summary>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> AsSingletons()
        {
            _alterations.Add(
                delegate(PluginFamily family) { family.InterceptionChain.AddInterceptor(new SingletonInterceptor()); });
            return this;
        }


        public CreatePluginFamilyExpression<PLUGINTYPE> OnCreation(StartupHandler<PLUGINTYPE> handler)
        {
            _alterations.Add(
                delegate(PluginFamily family) { family.InstanceInterceptor = new StartupInterceptor<PLUGINTYPE>(handler); });

            return this;
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> EnrichWith(EnrichmentHandler<PLUGINTYPE> handler)
        {
            _alterations.Add(
                delegate(PluginFamily family)
                    {
                        family.InstanceInterceptor = new EnrichmentInterceptor<PLUGINTYPE>(handler);
                    });

            return this;
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> AddConcreteType<CONCRETETYPE>()
        {
            return AddConcreteType<CONCRETETYPE>(Guid.NewGuid().ToString());
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> AddConcreteType<CONCRETETYPE>(string instanceName)
        {
            _alterations.Add(
                delegate(PluginFamily family)
                    {
                        Plugin plugin = Plugin.CreateImplicitPlugin(typeof (CONCRETETYPE));
                        plugin.ConcreteKey = instanceName;
                        family.Plugins.Add(plugin);
                    }
                );

            return this;
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> InterceptConstructionWith(InstanceFactoryInterceptor interceptor)
        {
            _alterations.Add(delegate(PluginFamily family) { family.InterceptionChain.AddInterceptor(interceptor); });
            return this;
        }
    }
}