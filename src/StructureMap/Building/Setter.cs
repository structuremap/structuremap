using System.Linq.Expressions;
using System.Reflection;

namespace StructureMap.Building
{
    public class Setter
    {
        private readonly MemberInfo _member;

        public Setter(MemberInfo member, IDependencySource value)
        {
            _member = member;
            AssignedValue = value;
        }

        public IDependencySource AssignedValue { get; private set; }

        public string Description { get; private set; }

        public MemberBinding ToBinding(ParameterExpression session)
        {
            return Expression.Bind(_member, AssignedValue.ToExpression(session));
        }
    }
}