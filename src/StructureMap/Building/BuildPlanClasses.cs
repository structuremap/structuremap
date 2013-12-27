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
     * IDependencySource Types
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


    public class InterceptionStep<T> : IDependencySource
    {
        private readonly InstanceInterceptor _interceptor;

        public InterceptionStep(InstanceInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        public IDependencySource Inner { get; set; }

        public InstanceInterceptor Interceptor
        {
            get { return _interceptor; }
        }

        public string Description
        {
            get { return _interceptor.ToString(); }
        }

        public Expression ToExpression(ParameterExpression session)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Wraps with direct access to the IContext.Transient stuff
    /// </summary>
    public class TransientStep : IDependencySource
    {
        public string Description { get; private set; }

        public Expression ToExpression(ParameterExpression session)
        {
            throw new NotImplementedException();
        }
    }

}