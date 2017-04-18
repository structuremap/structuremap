using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.Building.Interception;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Building
{
    public static class ConcreteType
    {
        public const string ConstructorArgument = "Constructor Argument";
        public const string SetterProperty = "Setter Property";
        public const string MissingPrimitiveWarning = "Required primitive dependency is not explicitly defined";
        public const string CastingError = "Could not convert value '{0}' of type {1} into the dependency type {2}";

        public const string UnableToDetermineDependency =
            "Unable to determine how to source dependency for type {0} and value '{1}'";

        public static ConcreteBuild BuildSource(Type pluggedType, ConstructorInfo constructor,
            DependencyCollection dependencies, Policies policies)
        {
            ConcreteBuild plan = null;

            try
            {
                var ctorStep = BuildConstructorStep(pluggedType, constructor, dependencies, policies);

                plan = new ConcreteBuild(pluggedType, ctorStep);

                determineSetterSources(pluggedType, dependencies, policies, plan);
            }
            catch (StructureMapException e)
            {
                e.Push("Attempting to create a build plan for concrete type " + pluggedType.GetFullName());
                throw;
            }
            catch (Exception e)
            {
                throw new StructureMapConfigurationException(
                    "Attempting to create a build plan for concrete type " + pluggedType.GetFullName(), e);
            }


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
            return pluggedType.GetSettableProperties().ToArray();
        }

        private static void determineSetterSource(DependencyCollection dependencies, Policies policies,
            PropertyInfo setter,
            IHasSetters plan)
        {
            var isMandatory = policies.IsMandatorySetter(setter);

            object dependency = null;
            if (setter.PropertyType.IsSimple() && !isMandatory)
            {
                dependency = dependencies.FindByTypeAndName(setter.PropertyType, setter.Name);
            }
            else
            {
                dependency = dependencies.FindByTypeOrName(setter.PropertyType, setter.Name);
            }

            if (dependency == null && !isMandatory) return;

            var source = SourceFor(policies, SetterProperty, setter.Name, setter.PropertyType, dependency);
            plan.Add(setter.PropertyType, setter, source);
        }

        public static ConstructorStep BuildConstructorStep(Type pluggedType, ConstructorInfo constructor,
            DependencyCollection dependencies, Policies policies)
        {
            var ctor = constructor ?? policies.SelectConstructor(pluggedType, dependencies);
            if (ctor == null)
            {
                throw new StructureMapConfigurationException(
                    "No public constructor could be selected for concrete type " + pluggedType.GetFullName());
            }

            var ctorStep = new ConstructorStep(ctor);
            var multiples = findTypesWithMultipleParametersRequired(ctor);

            var ctorDependencies = ctor
                .GetParameters()
                .Select(x => {
                    var dependency = multiples.Contains(x.ParameterType)
                        ? dependencies.FindByTypeAndName(x.ParameterType, x.Name)
                        : dependencies.FindByTypeOrName(x.ParameterType, x.Name);

                    if (dependency == null && ( (x.DefaultValue != null && x.DefaultValue.GetType().Name != "DBNull")))
                    {
                        dependency = x.DefaultValue;
                    }

                    return SourceFor(policies, ConstructorArgument, x.Name, x.ParameterType, dependency);
                });

            ctorStep.Add(ctorDependencies);

            return ctorStep;
        }

        private static Type[] findTypesWithMultipleParametersRequired(ConstructorInfo ctor)
        {
            var multiples = ctor.GetParameters().GroupBy(x => x.ParameterType)
                .Where(x => x.Count() > 1).Select(x => x.Key).ToArray();
            return multiples;
        }

        public static IDependencySource SourceFor(Policies policies, string ctorOrSetter, string name, Type dependencyType, object value)
        {
            if (value == null)
            {
                if (dependencyType.IsSimple())
                {
                    return new DependencyProblem
                    {
                        Message = MissingPrimitiveWarning,
                        Type = ctorOrSetter,
                        Name = name,
                        ReturnedType = dependencyType
                    };
                }

                return new DefaultDependencySource(dependencyType);
            }

            if (value is IDependencySource)
            {
                return value as IDependencySource;
            }

            if (value is Instance)
            {
                var instance = value.As<Instance>();
                if (instance.Interceptors.Any())
                {
                    var inner = instance.ToDependencySource(dependencyType);
                    return new InterceptionPlan(dependencyType, inner, policies, instance.Interceptors);
                }
                else
                {
                    return instance.ToDependencySource(dependencyType);
                }


            }

            if (value.GetType().CanBeCastTo(dependencyType))
            {
                return new Constant(dependencyType, value);
            }

            if (dependencyType.IsSimple())
            {
                try
                {
                    return new Constant(dependencyType, ConvertType(value, dependencyType));
                }
                catch (Exception)
                {
                    return new DependencyProblem
                    {
                        Type = ctorOrSetter,
                        Name = name,
                        ReturnedType = dependencyType,
                        Message =
                            CastingError.ToFormat(value, value.GetType().GetFullName(), dependencyType.GetFullName())
                    };
                }
            }

            if (EnumerableInstance.IsEnumerable(dependencyType))
            {
                var coercion = EnumerableInstance.DetermineCoercion(dependencyType);
                var coercedValue = coercion.Convert(value.As<IEnumerable<object>>());

                return new Constant(dependencyType, coercedValue);
            }


            return new DependencyProblem
            {
                Type = ctorOrSetter,
                Name = name,
                ReturnedType = dependencyType,
                Message = UnableToDetermineDependency.ToFormat(dependencyType.GetFullName(), value)
            };
        }

        public static object ConvertType(object value, Type dependencyType)
        {
            if (value.GetType() == dependencyType) return value;

            if (dependencyType.GetTypeInfo().IsEnum) return Enum.Parse(dependencyType, value.ToString());

            return Convert.ChangeType(value, dependencyType);
        }
    }
}