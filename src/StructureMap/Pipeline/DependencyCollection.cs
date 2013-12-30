using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Building;

namespace StructureMap.Pipeline
{
    /// <summary>
    /// Dumb class used to store inline dependencies.  Does NO
    /// validation of any sort on the Add() methods
    /// </summary>
    public class DependencyCollection
    {
        private readonly Stack<Argument> _dependencies = new Stack<Argument>();


        public object FindByTypeOrName(Type argumentType, string name)
        {
            var argument = _dependencies.FirstOrDefault(x => x.Name == name && x.Type == argumentType)
                           ?? _dependencies.FirstOrDefault(x => x.Type == argumentType)
                           ?? _dependencies.FirstOrDefault(x => x.Name == name);

            return argument == null ? null : argument.Dependency;
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
            _dependencies.Push(new Argument
            {
                Name = name,
                Type = type,
                Dependency = @dependency
            });
        }

        public class Argument
        {
            public string Name;
            public Type Type;
            public object Dependency;
        }

    }
}