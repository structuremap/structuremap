using System;

namespace StructureMap.Testing.Performance
{
    public class TypeMap
    {
        public TypeMap(Type interfaceType, Type implementationType)
        {
            InterfaceType = interfaceType;
            ImplementationType = implementationType;
        }

        public Type InterfaceType { get; }

        public Type ImplementationType { get; }

        public Tuple<Type, Type> AsTuple => Tuple.Create(InterfaceType, ImplementationType);
    }
}