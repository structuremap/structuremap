using System.Linq.Expressions;

namespace StructureMap.Building
{
    public interface IDependencySource
    {
        string Description { get; }
        Expression ToExpression(ParameterExpression session);
    }
}