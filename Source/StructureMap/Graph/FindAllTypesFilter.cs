using System;

namespace StructureMap.Graph
{
    public class FindAllTypesFilter : TypeRules, ITypeScanner
    {
        private readonly Type _pluginType;
        private Func<Type, string> _getName = type => type.FullName;

        public FindAllTypesFilter(Type pluginType)
        {
            _pluginType = pluginType;
        }

        #region ITypeScanner Members

        public void NameBy(Func<Type, string> getName)
        {
            _getName = getName;
        }

        public void Process(Type type, PluginGraph graph)
        {
            if (!type.CanBeCastTo(_pluginType)) return;

            var name = _getName(type);
            graph.AddType(_pluginType, type, name);
        }

        #endregion
    }
}