using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public class GenericConnectionScanner : ConfigurableRegistrationConvention
    {
        private readonly Type _openType;
        private readonly IList<Type> _interfaces = new List<Type>();
        private readonly IList<Type> _concretions = new List<Type>(); 

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
            var interfaceTypes = type.FindInterfacesThatClose(_openType);
            if (!interfaceTypes.Any()) return;

            if (type.IsConcrete())
            {
                _concretions.Add(type);
            }

            foreach (var interfaceType in interfaceTypes)
            {
                _interfaces.Add(interfaceType);
            }
        }

        public void Apply(PluginGraph graph)
        {
            var registry = new Registry();

            _interfaces.Each(@interface => {
                var expression = registry.For(@interface);
                ConfigureFamily(expression);
                
                _concretions.Where(x => x.CanBeCastTo(@interface)).Each(type => expression.Add(type));
            });

            registry.As<IPluginGraphConfiguration>().Configure(graph);
        }
    }
}