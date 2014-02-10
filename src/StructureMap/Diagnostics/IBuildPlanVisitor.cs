using System.Reflection;
using StructureMap.Building;
using StructureMap.Building.Interception;

namespace StructureMap.Diagnostics
{
    public interface IBuildPlanVisitor
    {
        void Constructor(ConstructorInfo constructor);
        void Parameter(ParameterInfo parameter, IDependencySource source);
        void Setter(Setter setter);

        void Activator(IInterceptor interceptor);
        void Decorator(IInterceptor interceptor);
    }
}