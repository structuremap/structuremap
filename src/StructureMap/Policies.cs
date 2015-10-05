using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.Building;
using StructureMap.Building.Interception;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap
{
    public class Policies
    {
        public readonly SetterRules SetterRules = new SetterRules();
        public readonly ConstructorSelector ConstructorSelector = new ConstructorSelector();

        private readonly object _buildLock = new object();

        private readonly IDictionary<Type, BuildUpPlan> _buildUpPlans
            = new Dictionary<Type, BuildUpPlan>();

        private readonly IList<IInstancePolicy> _policies = new List<IInstancePolicy>();

        public Policies()
        {
            _policies.Add(ConstructorSelector);
        }

        public void Add(IInstancePolicy policy)
        {
            _policies.Fill(policy);
        }

        public void Add(IInterceptorPolicy interception)
        {
            if (Interception().Contains(interception)) return;

            _policies.Add(new InterceptionPolicy(interception));
        }

        public IEnumerable<IInterceptorPolicy> Interception()
        {
            return _policies.OfType<InterceptionPolicy>().Select(x => x.Inner);
        }

        public void Apply(Type pluginType, Instance instance)
        {
            _policies.Where(x => !instance.AppliedPolicies.Contains(x))
                .Each(policy =>
                {
                    try
                    {
                        policy.Apply(pluginType, instance);
                    }
                    finally
                    {
                        instance.AppliedPolicies.Add(policy);
                    }
                });
        }

        public BuildUpPlan ToBuildUpPlan(Type pluggedType, Func<IConfiguredInstance> findInstance)
        {
            if (!_buildUpPlans.ContainsKey(pluggedType))
            {
                lock (_buildLock)
                {
                    if (!_buildUpPlans.ContainsKey(pluggedType))
                    {
                        var instance = findInstance();
                        var plan = ConcreteType.BuildUpPlan(pluggedType, instance.Dependencies, this);
                        _buildUpPlans.Add(pluggedType, plan);
                    }
                }
            }

            return _buildUpPlans[pluggedType];
        }

        public bool CanBeAutoFilled(Type concreteType)
        {
            var ctor = SelectConstructor(concreteType);

            if (ctor == null) return false;

            foreach (var parameter in ctor.GetParameters())
            {
                if (!parameter.ParameterType.IsAutoFillable() && (parameter.DefaultValue != null && parameter.DefaultValue.GetType().Name == "DBNull"))
                {
                    return false;
                }
            }

            var mandatories = ConcreteType.GetSetters(concreteType).Where(IsMandatorySetter).ToArray();
            if (mandatories.Any())
            {
                return mandatories.All(x => x.PropertyType.IsAutoFillable());
            }

            return true;
        }

        public bool IsMandatorySetter(PropertyInfo propertyInfo)
        {
            return SetterRules.IsMandatory(propertyInfo);
        }

        public ConstructorInfo SelectConstructor(Type pluggedType)
        {
            return ConstructorSelector.Select(pluggedType);
        }
    }
}