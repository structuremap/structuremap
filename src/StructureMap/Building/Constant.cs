using System;
using System.Linq.Expressions;

namespace StructureMap.Building
{
    public class Constant : IDependencySource
    {
        private readonly Type _argumentType;
        private readonly object _value;


        public static Constant ForObject(object o)
        {
            return new Constant(o.GetType(), o);
        }

        public static Constant For<T>(T value)
        {
            return new Constant(typeof (T), value);
        }

        public Constant(Type argumentType, object value)
        {
            _argumentType = argumentType;
            _value = value;
        }

        public Type ReturnedType
        {
            get { return _argumentType; }
        }

        public object Value
        {
            get { return _value; }
        }

        public string Description
        {
            get
            {
                return "Value: {0}".ToFormat(_value);

            }
        }

        public Expression ToExpression(ParameterExpression session)
        {
            return Expression.Constant(_value, _argumentType);
        }

        protected bool Equals(Constant other)
        {
            return Equals(_argumentType, other._argumentType) && Equals(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Constant) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_argumentType != null ? _argumentType.GetHashCode() : 0)*397) ^
                       (_value != null ? _value.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("ReturnedType: {0}, Value: {1}", _argumentType, _value);
        }
    }
}