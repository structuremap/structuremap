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
                throw new StructureMapConfigurationException("Attempting to create a build plan for concrete type " + pluggedType.GetFullName(), e);
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

            var source = SourceFor(SetterProperty, setter.Name, setter.PropertyType, dependency);
            plan.Add(setter, source);
        }

        public static ConstructorStep BuildConstructorStep(Type pluggedType, ConstructorInfo constructor,
            DependencyCollection dependencies, Policies policies)
        {
            var ctor = constructor ?? policies.SelectConstructor(pluggedType);
            if (ctor == null)
            {
                throw new StructureMapConfigurationException("No public constructor could be selected for concrete type " + pluggedType.GetFullName());
            }

            var ctorStep = new ConstructorStep(ctor);
            var ctorDependencies = ctor
                .GetParameters()
                .Select(x => {
                    var dependency = dependencies.FindByTypeOrName(x.ParameterType, x.Name);
                    return SourceFor(ConstructorArgument, x.Name, x.ParameterType, dependency);
                });

            ctorStep.Add(ctorDependencies);

            return ctorStep;
        }

        public static IDependencySource SourceFor(string ctorOrSetter, string name, Type dependencyType, object value)
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
                try
                {
                    return new Constant(dependencyType, ConvertType(value, dependencyType));
                }
                catch (Exception e)
                {
                    return new DependencyProblem
                    {
                        Type = ctorOrSetter,
                        Name = name,
                        ReturnedType = dependencyType,
                        Message = CastingError.ToFormat(value, value.GetType().GetFullName(), dependencyType.GetFullName())
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

            if (dependencyType.IsEnum) return Enum.Parse(dependencyType, value.ToString());

            return Convert.ChangeType(value, dependencyType);
        }
    }
}