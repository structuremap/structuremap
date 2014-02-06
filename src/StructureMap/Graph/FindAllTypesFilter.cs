using System;
using System.Reflection;
using StructureMap.Configuration.DSL;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public class FindAllTypesFilter : IRegistrationConvention
    {
        private readonly Type _pluginType;
        private Func<Type, string> _getName = type => Guid.NewGuid().ToString();

        public FindAllTypesFilter(Type pluginType)
        {
            _pluginType = pluginType;
        }

        public void Process(Type type, Registry registry)
        {
            if (type.CanBeCastTo(_pluginType) && type.HasConstructors())
            {
                var name = _getName(type);
                registry.AddType(GetLeastSpecificButValidType(_pluginType, type), type, name);
            }
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