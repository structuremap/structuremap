using System;
using StructureMap.Configuration.DSL;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public class GenericConnectionScanner : ConfigurableRegistrationConvention
    {
        private readonly Type _openType;

        public GenericConnectionScanner(Type openType)
        {
            _openType = openType;

            if (!_openType.IsOpenGeneric())
            {
                throw new ApplicationException("This scanning convention can only be used with open generic types");
            }
        }

        public override void Process(Type type, Registry registry)
        {
            var interfaceTypes = type.FindInterfacesThatClose(_openType);
            foreach (var interfaceType in interfaceTypes)
            {
                var family = registry.For(interfaceType);
                ConfigureFamily(family);
                family.Add(type);
            }
        }
    }
}