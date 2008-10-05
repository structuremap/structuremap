using System;

namespace StructureMap.Graph
{
    public class PluggableAttributeScanner : ITypeScanner
    {
        #region ITypeScanner Members

        public void Process(Type type, PluginGraph graph)
        {
            if (PluggableAttribute.MarkedAsPluggable(type))
            {
                graph.AddType(type);
            }
        }

        #endregion
    }
}