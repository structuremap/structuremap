using System;

namespace StructureMap.Graph
{
    public interface IPluginGraph
    {
        /// <summary>
        ///   Adds the concreteType as an Instance of the pluginType
        /// </summary>
        /// <param name = "pluginType"></param>
        /// <param name = "concreteType"></param>
        void AddType(Type pluginType, Type concreteType);

        /// <summary>
        ///   Adds the concreteType as an Instance of the pluginType with a name
        /// </summary>
        /// <param name = "pluginType"></param>
        /// <param name = "concreteType"></param>
        /// <param name = "name"></param>
        void AddType(Type pluginType, Type concreteType, string name);
    }
}