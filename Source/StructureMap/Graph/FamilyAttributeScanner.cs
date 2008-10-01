using System;

namespace StructureMap.Graph
{
    public class FamilyAttributeScanner : ITypeScanner
    {
        public void Process(Type type, PluginGraph graph)
        {
            if (PluginFamilyAttribute.MarkedAsPluginFamily(type))
            {
                graph.CreateFamily(type);
            }
        }
    }
}