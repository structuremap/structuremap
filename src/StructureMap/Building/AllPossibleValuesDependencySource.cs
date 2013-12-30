using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.Pipeline;

namespace StructureMap.Building
{
    public class AllPossibleValuesDependencySource : IDependencySource
    {
        public static readonly MethodInfo SessionMethod = typeof (IContext).GetMethod("GetAllInstances", new Type[0]);
        public static readonly MethodInfo ToArrayMethod = typeof(Enumerable).GetMethod("ToArray");


        private readonly Type _enumerationType;
        private readonly Type _elementType;

        public AllPossibleValuesDependencySource(Type enumerationType)
        {
            _enumerationType = enumerationType;
            _elementType = EnumerableInstance.DetermineElementType(enumerationType);
        }

        public string Description { get; private set; }

        public Expression ToExpression(ParameterExpression session)
        {
            var getData = Expression.Call(session, SessionMethod.MakeGenericMethod(_elementType));

            if (_enumerationType.IsArray || _enumerationType.GetGenericTypeDefinition() == typeof(IList<>))
            {
                return Expression.Call(null, ToArrayMethod.MakeGenericMethod(_elementType), getData);
            }
            
            if (_enumerationType.GetGenericTypeDefinition() != typeof (List<>))
            {
                return getData;
            }

            return ListDependencySource.ToExpression(_elementType, getData);
        }

        public Type EnumerationType
        {
            get { return _enumerationType; }
        }

        public Type ElementType
        {
            get { return _elementType; }
        }

        protected bool Equals(AllPossibleValuesDependencySource other)
        {
            return Equals(_enumerationType, other._enumerationType) && Equals(_elementType, other._elementType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AllPossibleValuesDependencySource) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_enumerationType != null ? _enumerationType.GetHashCode() : 0)*397) ^ (_elementType != null ? _elementType.GetHashCode() : 0);
            }
        }
    }
}