using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using System.Linq;

namespace StructureMap.Building
{
    /*
     * More Notes --
     * - Wrap each step in an exception wrapper
     * 
     * 
     * -- can get into BuildSession and make much more of the CPS stuff pre-determined
     * -- can get much more intelligent about validation
     * 
     */


    public class BuildPlan
    {
        private readonly Instance _instance;

        public BuildPlan(Instance instance)
        {
            _instance = instance;
        }
    }


    /*
     * IBuildStep Types
     * 1.) Instance node
     *   a.) ConstructorStep
     *   b.) ObjectNode
     *   c.) InstanceNode <-- just uses the old Instance.Build() signature
     * 2.) Constructor args
     *   a.) Build another instance
     *   b.) Inline argument
     * 3.) Setter values
     *   a.) Inline argument
     *   b.) Build another instance
     * 4.) Lifecycle activation
     *   a.) If it's "Unique", inline it
     *   b.) If it's singleton, go build it and then inline it
     *   c.) If it's transient, fetch from Transient cache on the IContext <--- Need to expose the transient cache on IContext
     *   d.) Anything else, that's the end of the line
     * 5.) Interceptor node
     *   a.) Stateless, no need to hit IContext |____ Not sure we need to differentiate
     *   b.) Stateful, needs to hit IContext    |
     */


    // TODO -- maybe make this much lighter.
    public interface IBuildStep
    {
        string Description { get; }

        Expression ToExpression();
    }

    public class InterceptionStep<T> : IBuildStep
    {
        private readonly InstanceInterceptor _interceptor;

        public InterceptionStep(InstanceInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        public IBuildStep Inner { get; set; }

        public InstanceInterceptor Interceptor
        {
            get { return _interceptor; }
        }

        public string Description
        {
            get { return _interceptor.ToString(); }
        }

        public Expression ToExpression()
        {
            throw new NotImplementedException();
        }
    }

    public class LiteralStep<T> : IBuildStep
    {
        private readonly T _object;
        private readonly string _description;

        public LiteralStep(T @object, string description)
        {
            _object = @object;
            _description = description;
        }

        public string Description
        {
            get { return _description; }
        }

        public Expression ToExpression()
        {
            throw new NotImplementedException();
        }
    }

    public class LifecycleStep : IBuildStep
    {
        private readonly ILifecycle _lifecycle;

        public LifecycleStep(ILifecycle lifecycle)
        {
            _lifecycle = lifecycle;
        }

        public string Description { get; private set; }
        public Expression ToExpression()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Wraps with direct access to the IContext.Transient stuff
    /// </summary>
    public class TransientStep : IBuildStep
    {


        public string Description { get; private set; }
        public Expression ToExpression()
        {
            throw new NotImplementedException();
        }
    }

    public class InstanceStep : IBuildStep
    {
        private readonly Type _pluginType;
        private readonly Instance _instance;

        public InstanceStep(Type pluginType, Instance instance)
        {
            _pluginType = pluginType;
            _instance = instance;
        }

        public string Description
        {
            get { return _instance.Description; }
        }

        public Instance Instance
        {
            get { return _instance; }
        }

        public Type PluginType
        {
            get { return _pluginType; }
        }

        public Expression ToExpression()
        {
            throw new NotImplementedException();
        }
    }

    /*
     * Needs to consist of a couple different things -->
     * 1.) 0..* IBuildStep's for constructor arguments
     * 2.) 0..* IBuildStep's for setters
     * 
     * - Will need to be knowledgeable about interceptors
     *   - no interceptors, then not much to do here
     *   - wrap interceptors if need be
     */
    public class ConstructorStep
    {
        private readonly ConstructorInfo _constructor;
        private readonly IList<IBuildStep> _arguments = new List<IBuildStep>(); 

        public ConstructorStep(ConstructorInfo constructor)
        {
            _constructor = constructor;
        }

        public void Add(IBuildStep argument)
        {
            _arguments.Add(argument);
        }

        public IEnumerable<IBuildStep> Arguments
        {
            get { return _arguments; }
        }

        public ConstructorInfo Constructor
        {
            get { return _constructor; }
        }

        public string Description
        {
            get { return _constructor.ToString(); } // TODO -- much more here
        }

        public NewExpression ToExpression()
        {
            return Expression.New(_constructor, _arguments.Select(x => x.ToExpression()));
        }


    }

    public class ConcreteBuild : IBuildStep
    {
        private readonly Type _concreteType;
        private readonly ConstructorStep _constructor;
        private readonly IList<Setter> _setters = new List<Setter>();

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
        }

        public Type ConcreteType
        {
            get { return _concreteType; }
        }

        public void Add(Setter setter)
        {
            _setters.Add(setter);
        }

        public void Add(MemberInfo member, IBuildStep value)
        {
            _setters.Add(new Setter(member, value));
        }

        public ConstructorStep Constructor
        {
            get { return _constructor; }
        }


        public string Description { get; private set; }


        public Delegate ToDelegate()
        {
            var inner = ToExpression();

            var lambdaType = typeof (Func<,>).MakeGenericType(typeof (IContext), _concreteType);
            var argument = Expression.Parameter(typeof (IContext), "c");
            
            var lambda = Expression.Lambda(lambdaType, inner, argument);

            return lambda.Compile();
        }

        public Expression ToExpression()
        {
            var newExpr = _constructor.ToExpression();
            
            if (!_setters.Any())
            {
                return newExpr;
            }
            
            return Expression.MemberInit(newExpr, _setters.Select(x => x.ToBinding()));
        }

        
    }

    public class ConcreteBuild<T> : ConcreteBuild
    {
        public static ConcreteBuild<T> For(Expression<Func<T>> expression)
        {
            var finder = new ConstructorFinderVisitor();
            finder.Visit(expression);

            ConstructorInfo ctor = finder.Constructor;

            return new ConcreteBuild<T>(ctor);
        } 

        public ConcreteBuild(ConstructorInfo constructor) : base(typeof(T), constructor)
        {
        }

        public ConcreteBuild() : base(typeof (T))
        {
        }

        public void Set<TValue>(Expression<Func<T, TValue>> expression, TValue value)
        {
            var member = ReflectionHelper.GetMember(expression);

            Add(new Setter(member, Constant.For(value)));
        }
    }


    public class Setter
    {
        private readonly MemberInfo _member;

        public Setter(MemberInfo member, IBuildStep value)
        {
            _member = member;
            AssignedValue = value;
        }

        public IBuildStep AssignedValue { get; private set; }

        public string Description { get; private set; }

        public MemberBinding ToBinding()
        {
            return Expression.Bind(_member, AssignedValue.ToExpression());
        }
    }

    public class Constant : IBuildStep
    {
        private readonly Type _argumentType;
        private readonly object _value;

        public static Constant For<T>(T value)
        {
            return new Constant(typeof(T), value);
        }

        public Constant(Type argumentType, object value)
        {
            _argumentType = argumentType;
            _value = value;
        }

        public string Description { get; private set; }
        public Expression ToExpression()
        {
            return Expression.Constant(_value, _argumentType);
        }
    }
}