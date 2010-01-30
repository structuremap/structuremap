using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Configuration.DSL;
using StructureMap.TypeRules;
using StructureMap.Util;

namespace StructureMap.Graph
{
    public class ImplementationMap : ConfigurableRegistrationConvention
    {
        private readonly Cache<Type, List<Type>> _types = new Cache<Type, List<Type>>(t => new List<Type>());

        public override void Process(Type type, Registry registry)
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
            var singleImplementationRegistry = new SingleImplementationRegistry();
            _types.Each((pluginType, types) =>
            {
                if (types.Count == 1)
                {
                    singleImplementationRegistry.AddType(pluginType, types[0]);
                    ConfigureFamily(singleImplementationRegistry.For(pluginType));
                }
            });
            singleImplementationRegistry.ConfigurePluginGraph(graph);
        }
    }

    internal class SingleImplementationRegistry : Registry
    {
        // This type created just to make the output clearer in WhatDoIHave()
        // might consider adding a Description property to Registry instead
    }
}