using System;
using System.Collections.Generic;
using StructureMap.Graph;

namespace StructureMap.Configuration.DSL
{
    public delegate void AlterPluginFamilyDelegate(PluginFamily family);

    public class CreatePluginFamilyExpression : IExpression
    {
        private Type _pluginType;
        private List<AlterPluginFamilyDelegate> _alterations = new List<AlterPluginFamilyDelegate>();
        private Plugin _lastPlugin;

        public CreatePluginFamilyExpression(Type pluginType)
        {
            _pluginType = pluginType;
        }



        public void Configure(PluginGraph graph)
        {
            PluginFamily family = PluginFamilyAttribute.CreatePluginFamily(_pluginType);
            foreach (AlterPluginFamilyDelegate alteration in _alterations)
            {
                alteration(family);
            }
            
            graph.PluginFamilies.Add(family);

            AssemblyGraph assembly = new AssemblyGraph(_pluginType.Assembly);
            graph.Assemblies.Add(assembly);
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

        public CreatePluginFamilyExpression PlugConcreteType<T>()
        {
            _lastPlugin = addPlugin<T>();

            return this;
        }

        public CreatePluginFamilyExpression WithAlias(string concreteKey)
        {
            _lastPlugin.ConcreteKey = concreteKey;
            return this;
        }
    }
}
