using System;
using System.Linq.Expressions;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

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
     * Next:
     * ConstructorStep w/ other build steps
     * Setter w/ other build steps
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

    public interface IBuildPlan
    {
        Delegate ToDelegate();
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


    public static class DelegateExtensions
    {
        public static Func<IContext, T> BuilderOf<T>(this Delegate @delegate)
        {
            return @delegate.As<Func<IContext, T>>();
        }

        public static Func<IContext, T> ToDelegate<T>(this IBuildPlan plan)
        {
            return plan.ToDelegate().As<Func<IContext, T>>();
        }
    }
}