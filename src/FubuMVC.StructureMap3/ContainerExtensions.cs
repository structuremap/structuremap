using System;
using Bottles;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace FubuMVC.StructureMap3
{
    public static class ContainerExtensions
    {
        public static void Activate<T>(this Registry registry, string description, Action<T> activation)
        {
            registry.For<IActivator>().Add<LambdaActivator<T>>()
                .Ctor<string>().Is(description)
                .Ctor<Action<T>>().Is(activation);
        }

        public static void Activate(this Registry registry, string description, Action activation)
        {
            registry.For<IActivator>().Add<LambdaActivator>()
                .Ctor<string>().Is(description)
                .Ctor<Action>().Is(activation);
        }

        public static void Activate(this IInitializationExpression expression, string description, Action activation)
        {
            expression.For<IActivator>().Add<LambdaActivator>()
                .Ctor<string>().Is(description)
                .Ctor<Action>().Is(activation);
        }
    }
}