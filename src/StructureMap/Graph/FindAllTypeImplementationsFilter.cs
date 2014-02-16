using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace Convo.Web.Graph
{
    public class FindAllTypeImplementationsFilter : IRegistrationConvention
    {
        private readonly Type _pluginType;
        private Func<Type, string> _getName = type => Guid.NewGuid().ToString();

        public FindAllTypeImplementationsFilter(Type pluginType)
        {
            _pluginType = pluginType;
        }

        public void Process(Type type, Registry registry)
        {
            if (type.CanBeCastTo(_pluginType)
                && type.GetConstructors().Any())
            {
                var name = _getName(type);
                foreach (var iface in GetAllValidTypes(_pluginType, type))
                {
                    registry.AddType(iface, type, name);
                }
            }
        }

        private IEnumerable<Type> GetAllValidTypes(Type pluginType, Type type)
        {
            return type.FindInterfacesThatClose(pluginType);
        }

        public void NameBy(Func<Type, string> getName)
        {
            _getName = getName;
        }
    }
}
