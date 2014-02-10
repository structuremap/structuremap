using System;
using System.Linq.Expressions;
using StructureMap.Diagnostics;
using StructureMap.TypeRules;

namespace StructureMap.Building
{
    public class DependencyProblem : IDependencySource
    {
        public string Name;
        public string Message;
        public string Type;

        public string Description
        {
            get { return "{0} '{1}' ({2}): {3}".ToFormat(Type, Name, ReturnedType.GetFullName(), Message); }
        }

        public Expression ToExpression(ParameterExpression session, ParameterExpression context)
        {
            throw new NotSupportedException(Description);
        }

        public Type ReturnedType { get; set; }
        public void AcceptVisitor(IDependencyVisitor visitor)
        {
            visitor.Problem(this);
        }
    }
}