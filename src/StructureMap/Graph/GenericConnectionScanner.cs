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

        public override string ToString()
        {
            return "Connect all implementations of open generic type " + _openType.GetTypeName();
        }

        public override void ScanTypes(TypeSet types, Registry registry)
        {
            types.AllTypes().Each(type =>
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
            });



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

            _concretions.Each(type => registry.Configure(graph => graph.ConnectedConcretions.Fill(type)));
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
                        // Because I'm too lazy to fight with the bleeping type constraints to "know"
                        // if it's possible to make the generic type and this is just easier.
                    }
                });
        }
    }
}