using System;
using System.Collections.Generic;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Interceptors;

namespace StructureMap.Configuration.DSL
{
    public delegate void AlterPluginFamilyDelegate(PluginFamily family);

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

        public CreatePluginFamilyExpression CacheBy(InstanceScope scope)
        {
            _alterations.Add(delegate(PluginFamily family)
                                 {
                                     InterceptorChainBuilder builder = new InterceptorChainBuilder();
                                     family.InterceptionChain = builder.Build(scope);
                                 });

            return this;
        }

        public CreatePluginFamilyExpression AsSingletons()
        {
            _alterations.Add(
                delegate(PluginFamily family) { family.InterceptionChain.AddInterceptor(new SingletonInterceptor()); });
            return this;
        }
    }
}