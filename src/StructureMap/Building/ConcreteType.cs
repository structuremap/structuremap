using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Building
{
    public static class ConcreteType
    {
        public static ConcreteBuild BuildSource(Type pluggedType, ConstructorInfo constructor,
            DependencyCollection dependencies, Policies policies)
        {
            var ctorStep = BuildConstructorStep(pluggedType, constructor, dependencies, policies);

            var plan = new ConcreteBuild(pluggedType, ctorStep);

            determineSetterSources(pluggedType, dependencies, policies, plan);

            return plan;
        }

        public static BuildUpPlan BuildUpPlan(Type pluggedType, DependencyCollection dependencies, Policies policies)
        {
            var plan = new BuildUpPlan(pluggedType);
            determineSetterSources(pluggedType, dependencies, policies, plan);

            return plan;
        }

        private static void determineSetterSources(Type pluggedType, DependencyCollection dependencies,
            Policies policies,
            IHasSetters plan)
        {
            var setters = GetSetters(pluggedType);

            setters.Each(setter => determineSetterSource(dependencies, policies, setter, plan));
        }

        public static PropertyInfo[] GetSetters(Type pluggedType)
        {
            return pluggedType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite && x.GetSetMethod(false) != null && x.GetSetMethod().GetParameters().Length == 1)
                .ToArray();
        }

        private static void determineSetterSource(DependencyCollection dependencies, Policies policies,
            PropertyInfo setter,
            IHasSetters plan)
        {
            var dependency = dependencies.FindByTypeOrName(setter.PropertyType, setter.Name);

            if (dependency == null && !policies.IsMandatorySetter(setter)) return;

            var source = SourceFor(setter.PropertyType, dependency);
            plan.Add(setter, source);
        }

        public static ConstructorStep BuildConstructorStep(Type pluggedType, ConstructorInfo constructor,
            DependencyCollection dependencies, Policies policies)
        {
            var ctor = constructor ?? policies.SelectConstructor(pluggedType);
            // TODO -- throw if doesn't exist

            var ctorStep = new ConstructorStep(ctor);
            var ctorDependencies = ctor
                .GetParameters()
                .Select(x => {
                    var dependency = dependencies.FindByTypeOrName(x.ParameterType, x.Name);
                    return SourceFor(x.ParameterType, dependency);
                });

            ctorStep.Add(ctorDependencies);
            return ctorStep;
        }

        public static IDependencySource SourceFor(Type dependencyType, object value)
        {
            if (value == null)
            {
                if (dependencyType.IsSimple())
                {
                    // TODO -- unit test this
                    throw new StructureMapConfigurationException("Missing a value for primitive dependency of type '{0}'", dependencyType.GetFullName());
                }

                if (EnumerableInstance.IsEnumerable(dependencyType))
                {
                    return new AllPossibleValuesDependencySource(dependencyType);
                }

                return new DefaultDependencySource(dependencyType);
            }

            if (value is Instance)
            {
                return value.As<Instance>().ToDependencySource(dependencyType);
            }

            if (value.GetType().CanBeCastTo(dependencyType))
            {
                return new Constant(dependencyType, value);
            }

            if (dependencyType.IsSimple())
            {
                return new Constant(dependencyType, ConvertType(value, dependencyType));
            }

            if (EnumerableInstance.IsEnumerable(dependencyType))
            {
                var coercion = EnumerableInstance.DetermineCoercion(dependencyType);
                var coercedValue = coercion.Convert(value.As<IEnumerable<object>>());

                return new Constant(dependencyType, coercedValue);
            }

            throw new NotSupportedException(
                "Unable to determine how to source dependency {0} and value '{1}'".ToFormat(dependencyType, value));
        }

        public static object ConvertType(object value, Type dependencyType)
        {
            if (value.GetType() == dependencyType) return value;

            if (dependencyType.IsEnum) return Enum.Parse(dependencyType, value.ToString());

            return Convert.ChangeType(value, dependencyType);
        }
    }

}