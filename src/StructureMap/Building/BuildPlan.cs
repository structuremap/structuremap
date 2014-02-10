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
    public class BuildPlan : IBuildPlan
    {
        private readonly Type _pluginType;
        private readonly Instance _instance;
        private readonly IDependencySource _inner;
        private readonly IInterceptionPlan _interceptionPlan;
        private readonly Func<IBuildSession, IContext, object> _func;


        public BuildPlan(Type pluginType, Instance instance, IDependencySource inner, Policies policies,
            IEnumerable<IInterceptor> interceptors)
        {
            _pluginType = pluginType;
            _instance = instance;
            _inner = inner;

            if (interceptors.Any())
            {
                _interceptionPlan = new InterceptionPlan(pluginType, _inner, policies, interceptors);
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
            _pluginType = pluginType;
            _instance = instance;
            _inner = inner;
            _interceptionPlan = interceptionPlan;
        }

        public Type PluginType
        {
            get { return _pluginType; }
        }

        public Instance Instance
        {
            get { return _instance; }
        }

        public IDependencySource Inner
        {
            get { return _inner; }
        }

        public IInterceptionPlan InterceptionPlan
        {
            get { return _interceptionPlan; }
        }

        public void AcceptVisitor(IBuildPlanVisitor visitor)
        {
            visitor.Instance(_pluginType, _instance);
            if (_interceptionPlan != null)
            {
                _interceptionPlan.AcceptVisitor(visitor);
            }

            var visitable = _inner.As<IBuildPlanVisitable>();
            if (visitable == null)
            {
                visitor.InnerBuilder(_inner);
            }
            else
            {
                visitable.AcceptVisitor(visitor);
            }

            
        }

        public Delegate ToDelegate()
        {
            var innerSource = _interceptionPlan ?? _inner;

            var builder = innerSource.ToExpression(Parameters.Session, Parameters.Context);

            if (builder.Type != _pluginType)
            {
                builder = Expression.Convert(builder, _pluginType);
            }

            var message =
                "Failure while building '{0}', check the inner exception for details".ToFormat(_instance.Description);
            var wrapped = TryCatchWrapper.WrapFunc<StructureMapBuildException>(message, _pluginType, builder, _instance);

            // Push/Pop for contextual construction and bi-directional dependency checking
            wrapped = PushPopWrapper.WrapFunc(_pluginType, _instance, wrapped);

            wrapped = TryCatchWrapper.WrapFunc<StructureMapBuildException>(message, _pluginType, wrapped, this);

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
                if (_instance.HasExplicitName())
                {
                    return "Instance of {0} ('{1}')".ToFormat(_pluginType.GetFullName(), _instance.Name);
                }

                return _instance.ReturnedType == null || _instance.ReturnedType == _pluginType
                    ? "Instance of {0}".ToFormat(_pluginType.GetFullName())
                    : "Instance of {0} ({1})".ToFormat(_pluginType.GetFullName(), _instance.ReturnedType.GetFullName());
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
                e.Instances.Add(_instance.Id);
                throw;
            }
        }

        public Type ReturnedType
        {
            get { return _pluginType; }
        }

    }
}