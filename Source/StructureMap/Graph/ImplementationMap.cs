using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.TypeRules;
using StructureMap.Util;

namespace StructureMap.Graph
{
    public class ImplementationMap : ITypeScanner
    {
        private readonly Cache<Type, List<Type>> _types = new Cache<Type, List<Type>>(t => new List<Type>());

        public void Process(Type type, PluginGraph graph)
        {
            RegisterType(type);
        }

        public void Register(Type interfaceType, Type concreteType)
        {
            _types[interfaceType].Add(concreteType);
        }

        public void RegisterType(Type type)
        {
            if (!type.CanBeCreated()) return;

            type.GetInterfaces().Where(i => i.IsVisible).Each(i => Register(i, type));
        }

        public void RegisterSingleImplementations(PluginGraph graph)
        {
            _types.Each((pluginType, types) =>
            {
                if (types.Count == 1)
                {
                    graph.AddType(pluginType, types[0]);
                }
            });
        }
    }
}