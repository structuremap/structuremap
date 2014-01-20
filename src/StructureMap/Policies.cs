using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using StructureMap.Building;
using StructureMap.Building.Interception;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.TypeRules;
using StructureMap.Util;

namespace StructureMap
{
    public class Policies
    {
        public readonly SetterRules SetterRules = new SetterRules();
        public readonly ConstructorSelector ConstructorSelector = new ConstructorSelector();
        public readonly InterceptorPolicies Interceptors = new InterceptorPolicies();

        private readonly object _buildLock = new object();
        private readonly IDictionary<Type, BuildUpPlan> _buildUpPlans 
            = new Dictionary<Type, BuildUpPlan>();

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
                if (!parameter.ParameterType.IsAutoFillable())
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