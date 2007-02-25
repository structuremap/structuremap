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
        private Plugin _lastPlugin;
        private InstanceScope _scope = InstanceScope.PerRequest;

        public CreatePluginFamilyExpression(Type pluginType)
        {
            _pluginType = pluginType;
        }



        public void Configure(PluginGraph graph)
        {
            PluginFamily family = PluginFamilyAttribute.CreatePluginFamily(_pluginType);
            InterceptorChainBuilder builder = new InterceptorChainBuilder();
            family.InterceptionChain = builder.Build(_scope);

            foreach (AlterPluginFamilyDelegate alteration in _alterations)
            {
                alteration(family);
            }
            
            graph.PluginFamilies.Add(family);

            AssemblyGraph assembly = new AssemblyGraph(_pluginType.Assembly);
            graph.Assemblies.Add(assembly);
        }

        IExpression[] IExpression.ChildExpressions
        {
            get { return new IExpression[0]; }
        }

        public CreatePluginFamilyExpression WithDefaultConcreteType<T>()
        {
            Plugin plugin = addPlugin<T>();

            _alterations.Add(delegate (PluginFamily family)
                                 {
                                     family.DefaultInstanceKey = plugin.ConcreteKey;
                                 });

            return this;
        }

        private Plugin addPlugin<T>()
        {
            Plugin plugin = Plugin.CreateImplicitPlugin(typeof (T));

            _alterations.Add(delegate (PluginFamily family)
                                 {
                                     family.Plugins.Add(plugin);
                                 });

            return plugin;
        }

        public CreatePluginFamilyExpression PluginConcreteType<T>()
        {
            _lastPlugin = addPlugin<T>();

            return this;
        }

        public CreatePluginFamilyExpression AliasedAs(string concreteKey)
        {
            _lastPlugin.ConcreteKey = concreteKey;
            return this;
        }

        public CreatePluginFamilyExpression AsASingleton()
        {
            _scope = InstanceScope.Singleton;
            return this;
        }

        public CreatePluginFamilyExpression CacheInstanceAtScope(InstanceScope scope)
        {
            _scope = scope;
            return this;
        }

        public void AndTheDefaultIs(InstanceExpression expression)
        {
            throw new NotImplementedException();
        }
    }
}
