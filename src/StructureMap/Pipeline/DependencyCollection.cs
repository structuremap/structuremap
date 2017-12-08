using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using StructureMap.TypeRules;

namespace StructureMap.Pipeline
{
    /// <summary>
    /// Dumb class used to store inline dependencies.  Does NO
    /// validation of any sort on the Add() methods
    /// </summary>
    public class DependencyCollection : IEnumerable<Argument>
    {
        private readonly List<Argument> _dependencies = new List<Argument>();

        /// <summary>
        /// Add a dependency for a constructor parameter (either a valueOrInstance of the parameter type or an Instance)
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="valueOrInstance"></param>
        public void AddForConstructorParameter(ParameterInfo parameter, object valueOrInstance)
        {
            Add(parameter.Name, parameter.ParameterType, valueOrInstance);
        }

        /// <summary>
        /// Add a dependency for a setter property
        /// </summary>
        /// <param name="property"></param>
        /// <param name="valueOrInstance"></param>
        public void AddForProperty(PropertyInfo property, object valueOrInstance)
        {
            Add(property.Name, property.PropertyType, valueOrInstance);
        }

        /// <summary>
        /// Finds the argument valueOrInstance (an Instance or a valueOrInstance of the right type) for the given argument type and name
        /// </summary>
        /// <param name="argumentType"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public object FindByTypeOrName(Type argumentType, string name)
        {
            Argument argument = null;

            if (argumentType.IsSimple())
            {
                argument = findByName(name)
                           ?? findByTypeOnly(argumentType);
            }
            else
            {
                argument = findByAll(argumentType, name)
                           ?? findByTypeOnly(argumentType)
                           ?? findByName(name);
            }

            return argument == null ? null : argument.Dependency;
        }

        private Argument findByName(string name)
        {
            if (name.IsEmpty()) return null;

            return _dependencies.LastOrDefault(x => x.Name == name);
        }


        private Argument findByTypeOnly(Type argumentType)
        {
            if (argumentType == null) return null;

            var argument = _dependencies.LastOrDefault(x => x.Type == argumentType);
            if (argument == null && EnumerableInstance.IsEnumerable(argumentType))
            {
                var elementType = EnumerableInstance.DetermineElementType(argumentType);
                argument = _dependencies
                    .LastOrDefault(x => x.Type == typeof (IEnumerable<>).MakeGenericType(elementType));
            }


            return argument;
        }

        private Argument findByAll(Type argumentType, string name)
        {
            if (argumentType == null) return null;

			var argument = _dependencies.LastOrDefault(x => x.Name == name &&
														(x.Type == argumentType ||
														isMatchOnNullableInnerType(argumentType, x.Type) ||
														(x.Type == null && argumentType.IsInstanceOfType(x.Dependency))));
			if (argument == null && EnumerableInstance.IsEnumerable(argumentType))
            {
                var elementType = EnumerableInstance.DetermineElementType(argumentType);
                argument = _dependencies
                    .LastOrDefault(x => x.Type == typeof (IEnumerable<>).MakeGenericType(elementType) && x.Name == name);
            }

            return argument;
        }

        private bool isMatchOnNullableInnerType(Type argumentType, Type dependencyType)
        {
            return argumentType.IsNullable() && dependencyType == argumentType.GetInnerTypeFromNullable();
        }

        /// <summary>
        /// Add a dependency for the type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueOrInstance"></param>
        public void Add<T>(T valueOrInstance)
        {
            Add(null, typeof (T), valueOrInstance);
        }

        /// <summary>
        /// Add a dependency for the type "T" as an Instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public void Add<T>(Instance instance)
        {
            Add(null, typeof (T), instance);
        }

        /// <summary>
        /// Add a dependency (valueOrInstance of "type" or Instance) for the given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dependency"></param>
        public void Add(Type type, object @dependency)
        {
            Add(null, type, dependency);
        }

        /// <summary>
        /// Add an enumerable of dependencies by parameter or property name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="items"></param>
        public void Add(string name, IEnumerable<Instance> items)
        {
            Add(name, null, new EnumerableInstance(items));
        }

        /// <summary>
        /// Add a dependency valueOrInstance by parameter or property name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dependency"></param>
        public void Add(string name, object @dependency)
        {
            Type type = null;
            if (@dependency != null && @dependency.GetType().IsSimple())
            {
                type = @dependency.GetType();
            }

            Add(name, type, dependency);
        }

        /// <summary>
        /// Insert an Argument into this dependency collection that will take precedence over 
        /// existing configuration
        /// </summary>
        /// <param name="argument"></param>
        public void Insert(Argument argument)
        {
            _dependencies.Insert(0, argument);
        }


        /// <summary>
        /// Add a dependency by parameter or property name and dependency type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="dependency"></param>
        public void Add(string name, Type type, object @dependency)
        {
            if (type.IsSimple())
            {
                if (@dependency == null)
                {
                    throw new ArgumentNullException("@dependency",
                        "Dependency valueOrInstance cannot be null for a simple argument of type '{1}' with name: '{0}".ToFormat(
                            name, type));
                }

                if (@dependency is LambdaInstance)
                {
                    if (@dependency.As<LambdaInstance>().ReturnedType != type)
                    {
                        throw new StructureMapConfigurationException(
                            "Invalid valueOrInstance '{0}' for parameter {1} of type {2}".ToFormat(@dependency, name,
                                type.GetFullName()));
                    }
                }
                else if (@dependency.GetType() != type)
                {
                    try
                    {
                        @dependency = Convert.ChangeType(@dependency, type, CultureInfo.InvariantCulture);
                    }
                    catch (Exception e)
                    {
                        throw new StructureMapConfigurationException(
                            "Invalid valueOrInstance '{0}' for parameter {1} of type {2}".ToFormat(@dependency, name,
                                type.GetFullName()), e);
                    }
                }
            }

            // Making sure you can override the default
            if (name.IsNotEmpty())
            {
                _dependencies.RemoveAll(x => x.Name == name);
            }
            else if (type != null)
            {
                _dependencies.RemoveAll(x => x.Type == type && x.Name.IsEmpty());
            }

            _dependencies.Add(new Argument
            {
                Name = name,
                Type = type,
                Dependency = @dependency
            });
        }

        public void RemoveByName(string name)
        {
            _dependencies.RemoveAll(x => x.Name == name);
        }

        public bool Has(string propertyName)
        {
            return _dependencies.Any(x => x.Name == propertyName);
        }

        public DependencyCollection Clone()
        {
            var peer = new DependencyCollection();
            CopyTo(peer);

            return peer;
        }

        public void CopyTo(DependencyCollection peer)
        {
            peer._dependencies.AddRange(_dependencies);
        }

        public IEnumerator<Argument> GetEnumerator()
        {
            return _dependencies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Argument argument)
        {
            _dependencies.Add(argument);
        }

        public object Get(string name)
        {
            if (!Has(name)) return null;

            var value = FindByTypeOrName(null, name);

            return value is ObjectInstance
                ? value.As<ObjectInstance>().Object
                : value;
        }

        public object FindByTypeAndName(Type propertyType, string name)
        {
            var arg = findByAll(propertyType, name);
            return arg == null ? null : arg.Dependency;
        }

        public bool HasAny()
        {
            return _dependencies.Any();
        }
    }
}