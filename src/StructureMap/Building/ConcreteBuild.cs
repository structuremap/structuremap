using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.Diagnostics;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Building
{
    public class ConcreteBuild : IHasSetters, IDependencySource, IBuildPlanVisitable
    {
        private readonly Type _concreteType;
        private readonly ConstructorStep _constructor;
        private readonly IList<Setter> _setters = new List<Setter>();

        public ConcreteBuild(Type concreteType) : this(concreteType, new ConstructorSelector().Select(concreteType))
        {
        }

        public ConcreteBuild(Type concreteType, ConstructorStep constructor)
        {
            _concreteType = concreteType;
            _constructor = constructor;
        }

        protected ConcreteBuild(Type concreteType, ConstructorInfo constructor)
            : this(concreteType, new ConstructorStep(constructor))
        {
        }


        public Type ConcreteType
        {
            get { return _concreteType; }
        }

        public void Add(Setter setter)
        {
            _setters.Add(setter);
        }

        public void Add(Type setterType, MemberInfo member, IDependencySource value)
        {
            _setters.Add(new Setter(setterType, member, value));
        }

        public ConstructorStep Constructor
        {
            get { return _constructor; }
        }

        public object Build(IBuildSession session, IContext context)
        {
            var @delegate = ToDelegate() as Func<IBuildSession, IContext, object>;

            return @delegate(session, context);
        }

        public T Build<T>(IBuildSession session) where T : class
        {
            return Build(session, session.As<IContext>()) as T;
        }

        public string Description
        {
            get
            {
                var args = _constructor.Arguments.Select(x => x.Dependency.Description).ToArray();

                var description = "new {0}({1})".ToFormat(_concreteType.Name, string.Join(", ", args));

                if (_setters.Any())
                {
                    description += "{";

                    _setters.Each(s => description += Environment.NewLine + "    " + s.Description);

                    description += Environment.NewLine + "}";
                }

                return description;
            }
        }


        public Delegate ToDelegate()
        {
            var inner = ToExpression(Parameters.Session, Parameters.Context);

            var lambdaType = typeof (Func<,,>).MakeGenericType(typeof (IBuildSession), typeof (IContext), _concreteType);

            var lambda = Expression.Lambda(lambdaType, inner, Parameters.Session, Parameters.Context);

            return lambda.Compile();
        }

        public Expression ToExpression(ParameterExpression session, ParameterExpression context)
        {
            var expression = buildInnerExpression(session, context);
            return
                TryCatchWrapper.WrapFunc<StructureMapBuildException>(
                    "Error while building type {0}.  See the inner exception for details".ToFormat(
                        _concreteType.GetFullName()), _concreteType, expression, this);
        }

        private Expression buildInnerExpression(ParameterExpression session, ParameterExpression context)
        {
            var newExpr = _constructor.ToExpression(session, context);

            if (!_setters.Any())
            {
                return newExpr;
            }


            return Expression.MemberInit(newExpr, _setters.Select(x => x.ToBinding(session, context)));
        }

        public Type ReturnedType
        {
            get { return _concreteType; }
        }

        public void AcceptVisitor(IDependencyVisitor visitor)
        {
            visitor.Concrete(this);
        }

        public bool IsValid()
        {
            if (_constructor.Arguments.Any(x => x.Dependency is DependencyProblem)) return false;

            if (_setters.Any(x => x.AssignedValue is DependencyProblem)) return false;

            return true;
        }

        public void AcceptVisitor(IBuildPlanVisitor visitor)
        {
            visitor.Constructor(_constructor);

            _setters.Each(visitor.Setter);
        }
    }

    public class ConcreteBuild<T> : ConcreteBuild, IDependencySource
    {
        public static ConcreteBuild<T> For(Expression<Func<T>> expression)
        {
            var finder = new ConstructorFinderVisitor();
            finder.Visit(expression);

            var ctor = finder.Constructor;

            return new ConcreteBuild<T>(ctor);
        }

        public ConcreteBuild(ConstructorInfo constructor)
            : base(typeof (T), constructor)
        {
        }

        public ConcreteBuild()
            : base(typeof (T))
        {
        }

        public void Set<TValue>(Expression<Func<T, TValue>> expression, TValue value)
        {
            var member = ReflectionHelper.GetMember(expression);

            Add(new Setter(typeof(TValue), member, Constant.For(value)));
        }

        public void Set(Expression<Func<T, object>> expression, IDependencySource step)
        {
            var property = ReflectionHelper.GetProperty(expression);
            Add(new Setter(property.PropertyType, property, step));
        }

        public ConcreteBuild<T> ConstructorArgs(params object[] args)
        {
            args.Each(a => {
                var arg = a as IDependencySource ?? Constant.ForObject(a);
                Constructor.Add(arg);
            });
            return this;
        }
    }
}