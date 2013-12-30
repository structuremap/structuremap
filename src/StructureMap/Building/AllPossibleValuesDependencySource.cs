using System;
using System.Linq.Expressions;

namespace StructureMap.Building
{
    public class AllPossibleValuesDependencySource : IDependencySource
    {
        private readonly Type _enumerationType;
        private readonly Type _elementType;

        public AllPossibleValuesDependencySource(Type enumerationType, Type elementType)
        {
            _enumerationType = enumerationType;
            _elementType = elementType;
        }

        public string Description { get; private set; }
        public Expression ToExpression(ParameterExpression session)
        {
            throw new NotImplementedException();
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