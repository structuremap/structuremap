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
        private readonly Lazy<Func<IBuildSession, object>> _func;


        public BuildPlan(Type pluginType, Instance instance, IDependencySource inner, IEnumerable<IInterceptor> interceptors)
        {
            _pluginType = pluginType;
            _instance = instance;
            _inner = inner;

            if (interceptors.Any())
            {
                _interceptionPlan = new InterceptionPlan(pluginType, _inner, interceptors);
            }



            _func = new Lazy<Func<IBuildSession, object>>(() => {
                var @delegate = ToDelegate();
                return @delegate as Func<IBuildSession, object>;
            });

        }

        public Delegate ToDelegate()
        {
            // TODO -- will add decorator later
            var innerSource = _interceptionPlan ?? _inner;

            var builder = innerSource.ToExpression(Parameters.Session);

            if (builder.Type != _pluginType)
            {
                builder = Expression.Convert(builder, _pluginType);
            }

            var wrapped = TryCatchWrapper.WrapFunc<StructureMapBuildException>(_pluginType, builder, _instance);
            wrapped = TryCatchWrapper.WrapFunc<StructureMapBuildException>(_pluginType, wrapped, this);

            // TODO -- add the bi-directional checking too

            var lambdaType = typeof (Func<,>).MakeGenericType(typeof (IBuildSession), _pluginType);

            var lambda = Expression.Lambda(lambdaType, wrapped, Parameters.Session);

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

                return _instance.ConcreteType == null || _instance.ConcreteType == _pluginType
                    ? "Instance of {0}".ToFormat(_pluginType.GetFullName())
                    : "Instance of {0} ({1})".ToFormat(_pluginType.GetFullName(), _instance.ConcreteType.GetFullName());

            }
        }

        public object Build(IBuildSession session)
        {
            return _func.Value(session);
        }

        public Expression ToExpression(ParameterExpression session)
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