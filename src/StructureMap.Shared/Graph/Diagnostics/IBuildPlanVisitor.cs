using System;
using System.Reflection;
using StructureMap.Building;
using StructureMap.Building.Interception;
using StructureMap.Pipeline;

namespace StructureMap.Diagnostics
{
    public interface IBuildPlanVisitor
    {
        void Constructor(ConstructorStep constructor);
        void Setter(Setter setter);

        void Activator(IInterceptor interceptor);
        void Decorator(IInterceptor interceptor);

        void Instance(Type pluginType, Instance instance);
        void InnerBuilder(IDependencySource inner);
    }
}