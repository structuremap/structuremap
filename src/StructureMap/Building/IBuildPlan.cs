using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using StructureMap.Building.Interception;
using StructureMap.Pipeline;

namespace StructureMap.Building
{
    public interface IBuildPlan : IDescribed
    {
        [Obsolete("Think this goes away")]
        object Build(IBuildSession session);

        Expression ToExpression(ParameterExpression session);

        Type ReturnedType { get; }
    }

    public class BuildPlan : IBuildPlan
    {
        // TODO -- make Instance implement IDescribed
        public BuildPlan(IDescribed instance, IDependencySource inner, IEnumerable<IInterceptor> interceptors)
        {
        }

        public string Description { get; private set; }
        public object Build(IBuildSession session)
        {
            throw new NotImplementedException();
        }

        public Expression ToExpression(ParameterExpression session)
        {
            throw new NotImplementedException();
        }

        public Type ReturnedType { get; private set; }
    }
}