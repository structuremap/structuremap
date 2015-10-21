using StructureMap.Pipeline;
using System;

namespace StructureMap.AutoFactory
{
    public interface IAutoFactoryMethodDefinition
    {
        AutoFactoryMethodType MethodType { get; }

        Type InstanceType { get; }

        string InstanceName { get; }

        ExplicitArguments ExplicitArguments { get; }
    }
}