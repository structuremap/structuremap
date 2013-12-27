using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.Graph;

namespace StructureMap.Building
{
    public class ConcreteBuild : IBuildPlan
    {
        private readonly Type _concreteType;
        private readonly ConstructorStep _constructor;
        private readonly IList<Setter> _setters = new List<Setter>();
        private readonly Lazy<Func<IBuildSession, object>> _func;


        public ConcreteBuild(Type concreteType) : this(concreteType, new Plugin(concreteType).GetConstructor())
        {
            
        }

        public ConcreteBuild(Type concreteType, ConstructorStep constructor)
        {
            _concreteType = concreteType;
            _constructor = constructor;
        }

        protected ConcreteBuild(Type concreteType, ConstructorInfo constructor) : this(concreteType, new ConstructorStep(constructor))
        {
            _func = new Lazy<Func<IBuildSession, object>>(() => {
                return ToDelegate() as Func<IBuildSession, object>;
            });
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

        public string Description { get; private set; }


        public Delegate ToDelegate()
        {
            var session = Expression.Parameter(typeof(IBuildSession), "session");
            var inner = ToExpression(session);

            var lambdaType = typeof (Func<,>).MakeGenericType(typeof (IBuildSession), _concreteType);
            
            
            var lambda = Expression.Lambda(lambdaType, inner, session);

            return lambda.Compile();
        }

        public Expression ToExpression(ParameterExpression session)
        {
            var newExpr = _constructor.ToExpression(session);
            
            if (!_setters.Any())
            {
                return newExpr;
            }
            
            return Expression.MemberInit(newExpr, _setters.Select(x => x.ToBinding(session)));
        }

        
    }

    public class ConcreteBuild<T> : ConcreteBuild, IDependencySource
    {
        public static ConcreteBuild<T> For(Expression<Func<T>> expression)
        {
            var finder = new ConstructorFinderVisitor();
            finder.Visit(expression);

            ConstructorInfo ctor = finder.Constructor;

            return new ConcreteBuild<T>(ctor);
        }

        public ConcreteBuild(ConstructorInfo constructor)
            : base(typeof(T), constructor)
        {
        }

        public ConcreteBuild()
            : base(typeof(T))
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