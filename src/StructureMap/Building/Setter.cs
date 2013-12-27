using System.Linq.Expressions;
using System.Reflection;

namespace StructureMap.Building
{
    public class Setter
    {
        private readonly MemberInfo _member;

        public Setter(MemberInfo member, IBuildStep value)
        {
            _member = member;
            AssignedValue = value;
        }

        public IBuildStep AssignedValue { get; private set; }

        public string Description { get; private set; }

        public MemberBinding ToBinding()
        {
            return Expression.Bind(_member, AssignedValue.ToExpression());
        }
    }
}