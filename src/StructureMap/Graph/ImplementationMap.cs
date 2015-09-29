using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Graph.Scanning;
using StructureMap.TypeRules;
using StructureMap.Util;

namespace StructureMap.Graph
{
    public class ImplementationMap : ConfigurableRegistrationConvention
    {
        private readonly LightweightCache<Type, List<Type>> _types = new LightweightCache<Type, List<Type>>(t => new List<Type>());

        public override void Process(Type type, Registry registry)
        {
            RegisterType(type);
        }

        public override Registry ScanTypes(TypeSet types)
        {
            var interfaces = types.FindTypes(TypeClassification.Interfaces);
            var concretes = types.FindTypes(TypeClassification.Concretes).Where(x => x.HasConstructors()).ToArray();

            var registry = new Registry();

            interfaces.Each(@interface =>
            {
                var implementors = concretes.Where(x => x.CanBeCastTo(@interface)).ToArray();
                if (implementors.Count() == 1)
                {
                    registry.AddType(@interface, implementors.Single());
                    ConfigureFamily(registry.For(@interface));
                }
            });



            return registry;
        }

        public void Register(Type interfaceType, Type concreteType)
        {
            _types[interfaceType].Add(concreteType);
        }

        public void RegisterType(Type type)
        {
            if (!type.CanBeCreated()) return;

            type.GetInterfaces().Where(i => i.GetTypeInfo().IsVisible).Each(i => Register(i, type));
        }

        public void RegisterSingleImplementations(PluginGraph graph)
        {
            var singleImplementationRegistry = new SingleImplementationRegistry();
            _types.Each((pluginType, types) => {
                if (types.Count == 1)
                {
                    singleImplementationRegistry.AddType(pluginType, types[0]);
                    ConfigureFamily(singleImplementationRegistry.For(pluginType));
                }
            });
            singleImplementationRegistry.As<IPluginGraphConfiguration>().Configure(graph);
        }
    }

    // This type created just to make the output clearer in WhatDoIHave()
    // might consider adding a Description property to Registry instead
    internal class SingleImplementationRegistry : Registry, IEquatable<SingleImplementationRegistry>
    {
        public bool Equals(SingleImplementationRegistry other)
        {
            if (ReferenceEquals(this, other)) return true;
            return false;
        }
    }
}