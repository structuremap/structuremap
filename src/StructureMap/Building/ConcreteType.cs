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
        public static IBuildPlan BuildPlan(Type pluggedType, ConstructorInfo constructor,
            DependencyCollection dependencies, Policies policies)
        {
            var ctorStep = BuildConstructorStep(pluggedType, constructor, dependencies, policies);

            var plan = new ConcreteBuild(pluggedType, ctorStep);

            determineSetterSources(pluggedType, dependencies, policies, plan);

            return plan;
        }

        private static void determineSetterSources(Type pluggedType, DependencyCollection dependencies,
            Policies policies,
            ConcreteBuild plan)
        {
            var setters = pluggedType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanWrite && x.GetSetMethod(false) != null && x.GetSetMethod().GetParameters().Length == 1)
                .ToArray();

            setters.Each(setter => determineSetterSource(dependencies, policies, setter, plan));
        }

        private static void determineSetterSource(DependencyCollection dependencies, Policies policies,
            PropertyInfo setter,
            ConcreteBuild plan)
        {
            var dependency = dependencies.FindByTypeOrName(setter.PropertyType, setter.Name);

            if (dependency == null && !policies.IsMandatorySetter(setter)) return;

            var source = SourceFor(setter.PropertyType, dependency);
            plan.Add(setter, source);
        }

        public static ConstructorStep BuildConstructorStep(Type pluggedType, ConstructorInfo constructor,
            DependencyCollection dependencies, Policies policies)
        {
            var ctor = constructor ?? policies.ConstructorSelector.Select(pluggedType);
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
                    // TODO -- needs to be a specific exception here.
                    throw new NotImplementedException();
                    //throw new StructureMapException(205, key, Name);
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
                var converter = TypeDescriptor.GetConverter(dependencyType);
                return new Constant(dependencyType, converter.ConvertFrom(value));
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
    }
}