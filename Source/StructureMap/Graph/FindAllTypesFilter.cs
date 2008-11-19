using System;

namespace StructureMap.Graph
{
    public class FindAllTypesFilter : TypeRules, ITypeScanner
    {
        private readonly Type _pluginType;

        public FindAllTypesFilter(Type pluginType)
        {
            _pluginType = pluginType;
        }

        #region ITypeScanner Members

        public void Process(Type type, PluginGraph graph)
        {


            if (CanBeCast(_pluginType, type))
            {
                graph.AddType(_pluginType, type);
            }
        }

        #endregion
    }
}