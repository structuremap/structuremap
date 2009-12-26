using System;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public class FindAllTypesFilter : ITypeScanner
    {
        private readonly Type _pluginType;
        private Func<Type, string> _getName = type => PluginCache.GetPlugin(type).ConcreteKey;

        public FindAllTypesFilter(Type pluginType)
        {
            _pluginType = pluginType;
        }

        #region ITypeScanner Members

        public void Process(Type type, PluginGraph graph)
        {
            if (type.CanBeCastTo(_pluginType) && Constructor.HasConstructors(type))
            {
                string name = _getName(type);
                graph.AddType(_pluginType, type, name);
            }
        }

        public void NameBy(Func<Type, string> getName)
        {
            _getName = getName;
        }

        #endregion
    }
}