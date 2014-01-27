using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
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

            var argument = _dependencies.LastOrDefault(x => x.Name == name && x.Type == argumentType);
            if (argument == null && EnumerableInstance.IsEnumerable(argumentType))
            {
                var elementType = EnumerableInstance.DetermineElementType(argumentType);
                argument = _dependencies
                    .LastOrDefault(x => x.Type == typeof (IEnumerable<>).MakeGenericType(elementType) && x.Name == name);
            }

            return argument;
        }

        public void Add<T>(T value)
        {
            Add(null, typeof (T), value);
        }

        public void Add<T>(Instance instance)
        {
            Add(null, typeof (T), instance);
        }

        public void Add(Type type, object @dependency)
        {
            Add(null, type, dependency);
        }

        public void Add(string name, IEnumerable<Instance> items)
        {
            Add(name, null, new EnumerableInstance(items));
        }

        public void Add(string name, object @dependency)
        {
            Add(name, null, dependency);
        }

        public void Insert(Argument argument)
        {
            _dependencies.Insert(0, argument);
        }


        public void Add(string name, Type type, object @dependency)
        {
            if (type.IsSimple())
            {
                if (@dependency == null)
                {
                    throw new ArgumentNullException("@dependency",
                        "Dependency value cannot be null for a simple argument of type '{1}' with name: '{0}".ToFormat(
                            name, type));
                }

                if (@dependency.GetType() != type)
                {
                    try
                    {
                        @dependency = Convert.ChangeType(@dependency, type, CultureInfo.InvariantCulture);
                    }
                    catch (Exception e)
                    {
                        throw new StructureMapConfigurationException("Invalid value '{0}' for parameter {1} of type {2}".ToFormat(@dependency, name, type.GetFullName()), e);
                    }
                }
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
    }
}