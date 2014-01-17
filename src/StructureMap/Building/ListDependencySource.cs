using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.TypeRules;

namespace StructureMap.Building
{
    public class ListDependencySource : ArrayDependencySource
    {
        public static readonly MethodInfo ToListMethod = typeof (Enumerable).GetMethod("ToList");

        public ListDependencySource(Type itemType, params IDependencySource[] items) : base(itemType, items)
        {
        }

        public override Expression ToExpression(ParameterExpression session)
        {
            var arrayExpression = base.ToExpression(session);
            var itemType = ItemType;

            return ToExpression(itemType, arrayExpression);
        }

        public static Expression ToExpression(Type itemType, Expression arrayExpression)
        {
            return Expression.Call(null, ToListMethod.MakeGenericMethod(itemType), arrayExpression);
        }

        public override string Description
        {
            get
            {
                return "List of all possible {0} values".ToFormat(ItemType.GetFullName());
            }
        }
    }
}