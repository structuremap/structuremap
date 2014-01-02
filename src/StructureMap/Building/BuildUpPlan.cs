using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace StructureMap.Building
{
    public class BuildUpPlan : IHasSetters
    {
        private readonly Type _concreteType;
        private readonly IList<Setter> _setters = new List<Setter>();
        private Lazy<ISetterActions> _buildups;

        public BuildUpPlan(Type concreteType)
        {
            _concreteType = concreteType;

            resetBuildups();
        }

        public Type ConcreteType
        {
            get { return _concreteType; }
        }

        private void resetBuildups()
        {
            _buildups = new Lazy<ISetterActions>(() => {
                var closedType = typeof (SetterActions<>).MakeGenericType(_concreteType);
                return Activator.CreateInstance(closedType, new object[] {_setters}).As<ISetterActions>();
            });
        }

        public IEnumerable<Setter> Setters
        {
            get { return _setters; }
        }

        public void Add(Setter setter)
        {
            _setters.Add(setter);

            if (_buildups.IsValueCreated)
            {
                resetBuildups();
            }
        }

        public void Add(MemberInfo member, IDependencySource value)
        {
            Add(new Setter(member, value));
        }

        public void BuildUp(IBuildSession session, object @object)
        {
            _buildups.Value.BuildUp(session, @object);
        }

        public interface ISetterActions
        {
            void BuildUp(IBuildSession session, object @object);
        }

        public class SetterActions<T> : ISetterActions where T : class
        {
            private readonly IEnumerable<Action<IBuildSession, T>> _actions;

            public SetterActions(IEnumerable<Setter> setters)
            {
                var target = Expression.Parameter(typeof (T), "target");
                _actions = setters.Select(x => {
                    var lambda = x.ToSetterLambda(typeof (T), target);
                    return lambda.Compile().As<Action<IBuildSession, T>>();
                }).ToArray();
            }

            public void BuildUp(IBuildSession session, object @object)
            {
                var target = @object.As<T>();
                _actions.Each(x => x(session, target));
            }
        }
    }


    public class BuildUpPlan<T> : BuildUpPlan
    {
        public BuildUpPlan() : base(typeof (T))
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
    }
}