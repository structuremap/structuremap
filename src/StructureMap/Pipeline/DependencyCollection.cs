using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Building;
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
                argument = findByName(name);
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

            return _dependencies.LastOrDefault(x => x.Type == argumentType);
        }

        private Argument findByAll(Type argumentType, string name)
        {
            if (argumentType == null) return null;

            return _dependencies.LastOrDefault(x => x.Name == name && x.Type == argumentType);
        }

        public void Add<T>(T value)
        {
            Add(null, typeof(T), value);
        }

        public void Add<T>(Instance instance)
        {
            Add(null, typeof(T), instance); 
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

        // TODO -- add some defensive programming checks here.
        // @dependency has to be null, castable to type, or Instance
        // check that Instance is pluggable
        public void Add(string name, Type type, object @dependency)
        {
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