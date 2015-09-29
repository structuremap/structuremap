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

        public bool Matches(Type type)
        {
            return type.CanBeCastTo(_pluginType) && type.HasConstructors();
        }

        public void ScanTypes(TypeSet types, Registry registry)
        {
            types.FindTypes(TypeClassification.Concretes).Where(Matches).Each(type =>
            {
                var name = _getName(type);
                registry.AddType(GetLeastSpecificButValidType(_pluginType, type), type, name);
            });
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