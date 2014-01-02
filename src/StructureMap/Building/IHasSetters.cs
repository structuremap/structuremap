using System;
using System.Reflection;

namespace StructureMap.Building
{
    public interface IHasSetters
    {
        Type ConcreteType { get; }
        void Add(Setter setter);
        void Add(MemberInfo member, IDependencySource value);
    }
}