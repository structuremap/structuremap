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
            var argument = _dependencies.LastOrDefault(x => x.Name == name && x.Type == argumentType)
                           ?? _dependencies.LastOrDefault(x => x.Type == argumentType)
                           ?? _dependencies.LastOrDefault(x => x.Name == name);

            return argument == null ? null : argument.Dependency;
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

        public void Add(string name, object @dependency)
        {
            Add(name, null, dependency);
        }

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
    }

    public class Argument
    {
        public string Name;
        public Type Type;
        public object Dependency;

        public Argument CloseType(params Type[] types)
        {
            var clone = new Argument
            {
                Name = Name
            };

            if (Type != null)
            {
                clone.Type = Type.IsOpenGeneric() ? Type.MakeGenericType(types) : Type;
            }

            if (Dependency is Instance)
            {
                clone.Dependency = Dependency.As<Instance>().CloseType(types) ?? Dependency;
            }
            else
            {
                clone.Dependency = Dependency;
            }

            return clone;
        }
    }
}