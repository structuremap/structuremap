using StructureMap.Pipeline;
using System;

namespace StructureMap.AutoFactory
{
    public class AutoFactoryMethodDefinition : IAutoFactoryMethodDefinition
    {
        public AutoFactoryMethodDefinition(AutoFactoryMethodType methodType, Type instanceType, string instanceName, ExplicitArguments explicitArguments)
        {
            MethodType = methodType;
            InstanceType = instanceType;
            InstanceName = instanceName;
            ExplicitArguments = explicitArguments;
        }

        public AutoFactoryMethodType MethodType { get; }

        public Type InstanceType { get; }

        public string InstanceName { get; }

        public ExplicitArguments ExplicitArguments { get; }
    }
}