using System;
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

        public void NameBy(Func<Type, string> getName)
        {
            _getName = getName;
        }

        public void Process(Type type, Registry registry)
        {
            if (type.CanBeCastTo(_pluginType) && Constructor.HasConstructors(type))
            {
                string name = _getName(type);
                registry.AddType(_pluginType, type, name);
            }
        }
    }
}