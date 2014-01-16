using System;
using System.Linq.Expressions;

namespace StructureMap.Building
{
    [Obsolete("Unnecessary now")]
    public class LiteralPlan<T> : IBuildPlan
    {
        private readonly T _object;
        private readonly string _description;

        public LiteralPlan(T @object)
        {
            _object = @object;
            
        }

        public LiteralPlan(T @object, string description)
        {
            _object = @object;
            _description = description;
        }

        public string Description
        {
            get { return _description ?? (_object == null ? "null" : _object.ToString()); }
        }

        public object Build(IBuildSession session)
        {
            return _object;
        }

        public Expression ToExpression(ParameterExpression session)
        {
            return Expression.Constant(_object);
        }

        public Type ReturnedType
        {
            get
            {
                return typeof (T);
            }
        }
    }
}