using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using StructureMap.Building.Interception;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Building
{

    public class BuildPlan : IBuildPlan
    {
        private readonly Type _pluginType;
        private readonly Instance _instance;
        private readonly IDependencySource _inner;
        private readonly InterceptionPlan _interceptionPlan;
        private readonly Lazy<Func<IBuildSession, IContext, object>> _func;


        public BuildPlan(Type pluginType, Instance instance, IDependencySource inner, IEnumerable<IInterceptor> interceptors)
        {
            _pluginType = pluginType;
            _instance = instance;
            _inner = inner;

            if (interceptors.Any())
            {
                _interceptionPlan = new InterceptionPlan(pluginType, _inner, interceptors);
            }



            _func = new Lazy<Func<IBuildSession, IContext, object>>(() => {
                var @delegate = ToDelegate();
                return @delegate as Func<IBuildSession, IContext, object>;
            });

        }

        public Delegate ToDelegate()
        {
            // TODO -- will add decorator later
            var innerSource = _interceptionPlan ?? _inner;

            var builder = innerSource.ToExpression(Parameters.Session, Parameters.Context);

            if (builder.Type != _pluginType)
            {
                builder = Expression.Convert(builder, _pluginType);
            }

            var wrapped = TryCatchWrapper.WrapFunc<StructureMapBuildException>(_pluginType, builder, _instance);
            
            // Push/Pop for contextual construction and bi-directional dependency checking
            wrapped = PushPopWrapper.WrapFunc(_pluginType,_instance, wrapped);
            
            wrapped = TryCatchWrapper.WrapFunc<StructureMapBuildException>(_pluginType, wrapped, this);

            if (!wrapped.Type.IsClass)
            {
                wrapped = Expression.Convert(wrapped, typeof(object));
            }

            var lambdaType = typeof (Func<,,>).MakeGenericType(typeof (IBuildSession), typeof(IContext), typeof(object));

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
            return _func.Value(session, context);
        }

        public Expression ToExpression(ParameterExpression session, ParameterExpression context)
        {
            throw new NotImplementedException();
        }

        public Type ReturnedType
        {
            get
            {
                return _pluginType;
            }
        }
    }
}