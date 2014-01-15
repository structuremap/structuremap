using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.Pipeline;

namespace StructureMap.Building
{
    public class ConcreteBuild : IBuildPlan, IHasSetters, IDependencySource
    {
        private readonly Type _concreteType;
        private readonly ConstructorStep _constructor;
        private readonly IList<Setter> _setters = new List<Setter>();
        private readonly Lazy<Func<IBuildSession, object>> _func;


        public ConcreteBuild(Type concreteType) : this(concreteType, new ConstructorSelector().Select(concreteType))
        {
        }

        public ConcreteBuild(Type concreteType, ConstructorStep constructor)
        {
            _concreteType = concreteType;
            _constructor = constructor;

            _func = new Lazy<Func<IBuildSession, object>>(() => {
                var @delegate = ToDelegate();
                return @delegate as Func<IBuildSession, object>;
            });
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

        public void Add(MemberInfo member, IDependencySource value)
        {
            _setters.Add(new Setter(member, value));
        }

        public ConstructorStep Constructor
        {
            get { return _constructor; }
        }

        public object Build(IBuildSession session)
        {
            return _func.Value(session);
        }

        public string Description
        {
            get
            {
                var args = _constructor.Arguments.Select(x => x.Description).ToArray();

                return "new {0}({1})".ToFormat(_concreteType.Name, string.Join(", ", args));
            }
        }


        public Delegate ToDelegate()
        {
            var inner = ToExpression(Parameters.Session);

            

            var lambdaType = typeof (Func<,>).MakeGenericType(typeof (IBuildSession), _concreteType);

            var lambda = Expression.Lambda(lambdaType, inner, Parameters.Session);

            return lambda.Compile();
        }

        public Expression ToExpression(ParameterExpression session)
        {
            var expression = buildInnerExpression(session);
            return TryCatchWrapper.WrapFunc<StructureMapBuildException>(_concreteType, expression, this);
        }

        private Expression buildInnerExpression(ParameterExpression session)
        {
            var newExpr = _constructor.ToExpression(session);

            if (!_setters.Any())
            {
                return newExpr;
            }


            return Expression.MemberInit(newExpr, _setters.Select(x => x.ToBinding(session)));
        }

        public Type ReturnedType
        {
            get
            {
                return _concreteType;
            }
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

            Add(new Setter(member, Constant.For(value)));
        }

        public void Set(Expression<Func<T, object>> expression, IDependencySource step)
        {
            var member = ReflectionHelper.GetMember(expression);
            Add(new Setter(member, step));
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