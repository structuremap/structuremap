using System.Linq.Expressions;

namespace StructureMap.Building
{
    public interface IDependencySource : IDescribed
    {
        Expression ToExpression(ParameterExpression session);
    }
}