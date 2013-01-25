using System;
using System.Linq;
using StructureMap.Configuration.DSL;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public class FindAllTypesFilter : IRegistrationConvention
    {
        private readonly Type _pluginType;
        private Func<Type, string> _getName = type => PluginCache.GetPlugin(type).ConcreteKey;

        public FindAllTypesFilter(Type pluginType)
        {
            _pluginType = pluginType;
        }

        public void Process(Type type, Registry registry)
        {
            if (type.CanBeCastTo(_pluginType) && Constructor.HasConstructors(type))
            {
                string name = _getName(type);
                registry.AddType(GetLeastSpecificButValidType(_pluginType, type), type, name);
            }
        }

        private Type GetLeastSpecificButValidType(Type pluginType, Type type)
        {
            if (pluginType.IsGenericTypeDefinition && !type.IsOpenGeneric())
                return type.FindFirstInterfaceThatCloses(pluginType);

            return pluginType;
        }

        public void NameBy(Func<Type, string> getName)
        {
            _getName = getName;
        }
    }
}