using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.Building.Interception;
using StructureMap.Diagnostics;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Building
{
#pragma warning disable 1591
    public class BuildPlan : IBuildPlan
    {
        private readonly IInterceptionPlan _interceptionPlan;
        private readonly Func<IBuildSession, IContext, object> _func;


        public BuildPlan(Type pluginType, Instance instance, IDependencySource inner, Policies policies,
            IEnumerable<IInterceptor> interceptors)
        {
            PluginType = pluginType;
            Instance = instance;
            Inner = inner;

            if (interceptors.Any())
            {
                _interceptionPlan = new InterceptionPlan(pluginType, Inner, policies, interceptors);
            }

            var @delegate = ToDelegate();
            _func = @delegate as Func<IBuildSession, IContext, object>;
        }

        /// <summary>
        /// FOR TESTING ONLY!
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="instance"></param>
        /// <param name="inner"></param>
        /// <param name="interceptionPlan"></param>
        public BuildPlan(Type pluginType, Instance instance, IDependencySource inner, IInterceptionPlan interceptionPlan)
        {
            PluginType = pluginType;
            Instance = instance;
            Inner = inner;
            _interceptionPlan = interceptionPlan;
        }

        public Type PluginType { get; }

        public Instance Instance { get; }

        public IDependencySource Inner { get; }

        public IInterceptionPlan InterceptionPlan => _interceptionPlan;

        public void AcceptVisitor(IBuildPlanVisitor visitor)
        {
            visitor.Instance(PluginType, Instance);
            _interceptionPlan?.AcceptVisitor(visitor);

            var visitable = Inner.As<IBuildPlanVisitable>();
            if (visitable == null)
            {
                visitor.InnerBuilder(Inner);
            }
            else
            {
                visitable.AcceptVisitor(visitor);
            }

            
        }

        public Delegate ToDelegate()
        {
            var innerSource = _interceptionPlan ?? Inner;

            var builder = innerSource.ToExpression(Parameters.Session, Parameters.Context);

            if (builder.Type != PluginType)
            {
                builder = Expression.Convert(builder, PluginType);
            }

            var message =
                "Failure while building '{0}', check the inner exception for details".ToFormat(Instance.Description);
            var wrapped = TryCatchWrapper.WrapFunc<StructureMapBuildException>(message, PluginType, builder, Instance);

            // Push/Pop for contextual construction and bi-directional dependency checking
            wrapped = PushPopWrapper.WrapFunc(PluginType, Instance, wrapped);

            wrapped = TryCatchWrapper.WrapFunc<StructureMapBuildException>(message, PluginType, wrapped, this);

            if (!wrapped.Type.GetTypeInfo().IsClass)
            {
                wrapped = Expression.Convert(wrapped, typeof (object));
            }

            var lambdaType = typeof (Func<,,>).MakeGenericType(typeof (IBuildSession), typeof (IContext),
                typeof (object));

            var lambda = Expression.Lambda(lambdaType, wrapped, Parameters.Session, Parameters.Context);

            return lambda.Compile();
        }

        public string Description
        {
            get
            {
                if (Instance.HasExplicitName())
                {
                    return "Instance of {0} ('{1}')".ToFormat(PluginType.GetFullName(), Instance.Name);
                }

                return Instance.ReturnedType == null || Instance.ReturnedType == PluginType
                    ? "Instance of {0}".ToFormat(PluginType.GetFullName())
                    : "Instance of {0} ({1})".ToFormat(PluginType.GetFullName(), Instance.ReturnedType.GetFullName());
            }
        }

        public object Build(IBuildSession session, IContext context)
        {
            try
            {
                return _func(session, context);
            }
            catch (StructureMapException e)
            {
                e.Instances.Add(Instance.Id);
                throw;
            }
        }

        public Type ReturnedType => PluginType;
    }
#pragma warning restore 1591
}