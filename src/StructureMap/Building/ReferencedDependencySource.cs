using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.TypeRules;

namespace StructureMap.Building
{
    public class ReferencedDependencySource : IDependencySource
    {
        private readonly Type _dependencyType;
        private readonly string _name;

        public static MethodInfo ContextMethod =
            typeof (IContext).GetMethods()
                .FirstOrDefault(x => x.Name == "GetInstance" && x.IsGenericMethod && x.GetParameters().Count() == 1);

        public ReferencedDependencySource(Type dependencyType, string name)
        {
            _dependencyType = dependencyType;
            _name = name;
        }

        public Type DependencyType
        {
            get { return _dependencyType; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return "Name='{0}'".ToFormat(_name); }
        }

        public Expression ToExpression(ParameterExpression session, ParameterExpression context)
        {
            return Expression.Call(context, ContextMethod.MakeGenericMethod(_dependencyType), Expression.Constant(_name));
        }

        public Type ReturnedType
        {
            get { return DependencyType; }
        }

        public override string ToString()
        {
            return string.Format("DependencyType: {0}, Name: {1}", _dependencyType, _name);
        }

        protected bool Equals(ReferencedDependencySource other)
        {
            return Equals(_dependencyType, other._dependencyType) && string.Equals(_name, other._name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ReferencedDependencySource) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_dependencyType != null ? _dependencyType.GetHashCode() : 0)*397) ^
                       (_name != null ? _name.GetHashCode() : 0);
            }
        }
    }
}