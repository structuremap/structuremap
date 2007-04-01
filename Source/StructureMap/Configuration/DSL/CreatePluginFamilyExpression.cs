using System;
using System.Collections.Generic;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Interceptors;

namespace StructureMap.Configuration.DSL
{
    public delegate void AlterPluginFamilyDelegate(PluginFamily family);

    /// <summary>
    /// Represents the parameters for creating instances of a given Type
    /// </summary>
    public class CreatePluginFamilyExpression : IExpression
    {
        private Type _pluginType;
        private List<AlterPluginFamilyDelegate> _alterations = new List<AlterPluginFamilyDelegate>();
        private InstanceScope _scope = InstanceScope.PerRequest;
        private List<IExpression> _children = new List<IExpression>();

        public CreatePluginFamilyExpression(Type pluginType)
        {
            _pluginType = pluginType;
        }

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

        /// <summary>
        /// Sets the default instance of a Type to the definition represented by builder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression TheDefaultIs(IMementoBuilder builder)
        {
            builder.ValidatePluggability(_pluginType);

            _children.Add(builder);
            _alterations.Add(delegate(PluginFamily family)
                                 {
                                     InstanceMemento memento = builder.BuildMemento(family);
                                     family.Source.AddExternalMemento(memento);
                                     family.DefaultInstanceKey = memento.InstanceKey;
                                 });

            return this;
        }

        /// <summary>
        /// Convenience method that sets the default concrete type of the PluginType.  Type T
        /// can only accept types that do not have any primitive constructor arguments.
        /// StructureMap has to know how to construct all of the constructor argument types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CreatePluginFamilyExpression TheDefaultIsConcreteType<T>()
        {
            ExpressionValidator.ValidatePluggabilityOf(typeof (T)).IntoPluginType(_pluginType);

            _alterations.Add(delegate(PluginFamily family)
                                 {
                                     Plugin plugin = family.Plugins.FindOrCreate(typeof (T));
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
        public CreatePluginFamilyExpression CacheBy(InstanceScope scope)
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
        public CreatePluginFamilyExpression AsSingletons()
        {
            _alterations.Add(
                delegate(PluginFamily family) { family.InterceptionChain.AddInterceptor(new SingletonInterceptor()); });
            return this;
        }
    }
}