using System;
using StructureMap.Building;

namespace StructureMap.Diagnostics
{
    public interface IDependencyVisitor
    {
        void Constant(Constant constant);
        void Default(Type pluginType);
        void Referenced(ReferencedDependencySource source);
        void InlineEnumerable(IEnumerableDependencySource source);
        void AllPossibleOf(Type pluginType);
        void Concrete(ConcreteBuild build);

        void Lifecycled(LifecycleDependencySource source);

        /// <summary>
        /// This is strictly for dependency source types that do not need any 
        /// special handling
        /// </summary>
        /// <param name="source"></param>
        void Dependency(IDependencySource source);

        void Problem(DependencyProblem problem);
    }
}