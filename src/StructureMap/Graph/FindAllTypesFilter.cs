using System;
using System.Linq;
using StructureMap.Configuration.DSL;
using StructureMap.Graph.Scanning;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public class FindAllTypesFilter : IRegistrationConvention
    {
        private readonly Type _pluginType;
        private Func<Type, string> _getName = type => null;

        public FindAllTypesFilter(Type pluginType)
        {
            _pluginType = pluginType;
        }

        public void Process(Type type, Registry registry)
        {
            if (type.IsAbstract) return;
            if (type.IsInterface) return;

            if (type.CanBeCastTo(_pluginType) && type.HasConstructors())
            {
                var name = _getName(type);
                registry.AddType(GetLeastSpecificButValidType(_pluginType, type), type, name);
            }
        }

        public bool Matches(Type type)
        {
            return type.CanBeCastTo(_pluginType) && type.HasConstructors();
        }

        public Registry ScanTypes(TypeSet types)
        {
            var registry = new Registry();

            types.FindTypes(TypeClassification.Concretes).Where(Matches).Each(type =>
            {
                var name = _getName(type);
                registry.AddType(GetLeastSpecificButValidType(_pluginType, type), type, name);
            });

            return registry;
        }

        private Type GetLeastSpecificButValidType(Type pluginType, Type type)
        {
            if (pluginType.GetTypeInfo().IsGenericTypeDefinition && !type.IsOpenGeneric())
                return type.FindFirstInterfaceThatCloses(pluginType);

            return pluginType;
        }

        public void NameBy(Func<Type, string> getName)
        {
            _getName = getName;
        }
    }
}