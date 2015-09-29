using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Graph.Scanning;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public class GenericConnectionScanner : ConfigurableRegistrationConvention
    {
        private readonly IList<Type> _concretions = new List<Type>();
        private readonly IList<Type> _interfaces = new List<Type>();
        private readonly Type _openType;

        public GenericConnectionScanner(Type openType)
        {
            _openType = openType;

            if (!_openType.IsOpenGeneric())
            {
                throw new InvalidOperationException("This scanning convention can only be used with open generic types");
            }
        }

        public override void Process(Type type, Registry registry)
        {
            IEnumerable<Type> interfaceTypes = type.FindInterfacesThatClose(_openType);
            if (!interfaceTypes.Any()) return;

            if (type.IsConcrete())
            {
                _concretions.Add(type);
            }

            foreach (Type interfaceType in interfaceTypes)
            {
                _interfaces.Fill(interfaceType);
            }
        }

        public override Registry ScanTypes(TypeSet types)
        {
            var registry = new Registry();

            var concretions = types.FindTypes(TypeClassification.Concretes)
                .Where(type => type.FindInterfacesThatClose(_openType).Any()).ToArray();

            var interfaces = types.FindTypes(TypeClassification.Interfaces)
                .SelectMany(type => type.FindInterfacesThatClose(_openType))
                .Distinct()
                .ToArray();

            interfaces.Each(@interface =>
            {
                var expression = registry.For(@interface);
                ConfigureFamily(expression);

                var exactMatches = concretions.Where(x => x.CanBeCastTo(@interface)).ToArray();
                if (exactMatches.Length == 1)
                {
                    expression.Use(exactMatches.Single());
                }
                else
                {
                    exactMatches.Each(type => expression.Add(type));
                }


                if (!@interface.IsOpenGeneric())
                {
                    addConcretionsThatCouldBeClosed(@interface, expression);
                }
            });

            concretions.Each(type => registry.Configure(graph => graph.ConnectedConcretions.Fill(type)));

            return registry;
        }

        public void Apply(PluginGraph graph)
        {
            var registry = new Registry();

            _interfaces.Each(@interface =>
            {
                var expression = registry.For(@interface);
                ConfigureFamily(expression);

                var exactMatches = _concretions.Where(x => x.CanBeCastTo(@interface)).ToArray();
                if (exactMatches.Length == 1)
                {
                    expression.Use(exactMatches.Single());
                }
                else
                {
                    exactMatches.Each(type => expression.Add(type));
                }


                if (!@interface.IsOpenGeneric())
                {
                    addConcretionsThatCouldBeClosed(@interface, expression);
                }
            });

            _concretions.Each(t => graph.ConnectedConcretions.Fill(t));
            registry.As<IPluginGraphConfiguration>().Configure(graph);
        }

        private void addConcretionsThatCouldBeClosed(Type @interface, GenericFamilyExpression expression)
        {
            _concretions.Where(x => x.IsOpenGeneric())
                .Where(x => x.CouldCloseTo(@interface))
                .Each(type =>
                {
                    try
                    {
                        expression.Add(
                            type.MakeGenericType(@interface.GetGenericArguments()));
                    }
                    catch (Exception)
                    {
                        // Because I'm too lazy to fight with the fucking type constraints to "know"
                        // if it's possible to make the generic type and this is just easier.
                    }
                });
        }
    }
}