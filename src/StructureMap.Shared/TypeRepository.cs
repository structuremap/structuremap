using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using StructureMap.TypeRules;
using StructureMap.Util;

namespace StructureMap
{
    // Really only tested in integration with other things
    public static class TypeRepository
    {
        private static readonly Cache<Assembly, Task<AssemblyTypes>> _assemblies = new Cache
            <Assembly, Task<AssemblyTypes>>(
            assem => { return Task.Factory.StartNew(() => new AssemblyTypes(assem)); });

        public static void ClearAll()
        {
            _assemblies.ClearAll();
        }

        public static Task<IEnumerable<Type>> FindTypes(IEnumerable<Assembly> assemblies,
            TypeClassification classification, Func<Type, bool> filter = null)
        {
            var query = new TypeQuery(classification, filter);

            var tasks = assemblies.Select(assem => _assemblies[assem].ContinueWith(t => query.Find(t.Result))).ToArray();
            return Task.Factory.ContinueWhenAll(tasks, results => results.SelectMany(x => x.Result));
        }

        public static Task<IEnumerable<Type>> FindTypes(Assembly assembly, TypeClassification classification,
            Func<Type, bool> filter = null)
        {
            var query = new TypeQuery(classification, filter);

            return _assemblies[assembly].ContinueWith(t => query.Find(t.Result));
        }
    }

    [Flags]
    public enum TypeClassification : short
    {
        All = 0,
        Open = 1,
        Closed = 2,
        Interfaces = 4,
        Abstracts = 8,
        Concretes = 16
    }

    public class TypeQuery
    {
        private readonly TypeClassification _classification;

        public TypeQuery(TypeClassification classification, Func<Type, bool> filter = null)
        {
            Filter = filter ?? (t => true);
            _classification = classification;
        }

        public readonly Func<Type, bool> Filter;

        public IEnumerable<Type> Find(AssemblyTypes assembly)
        {
            return assembly.FindTypes(_classification).Where(Filter);
        }
    }

    public class AssemblyTypes
    {
        public AssemblyTypes(Assembly assembly) : this(assembly.GetExportedTypes())
        {
        }

        public AssemblyTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                var shelf = type.IsOpenGeneric() ? OpenTypes : ClosedTypes;
                shelf.Add(type);
            }
        }

        public readonly AssemblyShelf ClosedTypes = new AssemblyShelf();
        public readonly AssemblyShelf OpenTypes = new AssemblyShelf();

        public IEnumerable<Type> FindTypes(TypeClassification classification)
        {
            if (classification == TypeClassification.All)
            {
                return ClosedTypes.AllTypes().Concat(OpenTypes.AllTypes());
            }

            if (classification == TypeClassification.Interfaces)
            {
                return allTypes(ClosedTypes.Interfaces, OpenTypes.Interfaces);
            }

            if (classification == TypeClassification.Abstracts)
            {
                return allTypes(ClosedTypes.Abstracts, OpenTypes.Abstracts);
            }

            if (classification == TypeClassification.Concretes)
            {
                return allTypes(ClosedTypes.Concretes, OpenTypes.Concretes);
            }

            if (classification == TypeClassification.Open)
            {
                return OpenTypes.AllTypes();
            }

            if (classification == TypeClassification.Closed)
            {
                return ClosedTypes.AllTypes();
            }

            return allTypes(selectGroups(classification).ToArray());
        }

        private IEnumerable<Type> allTypes(params IList<Type>[] shelves)
        {
            return shelves.SelectMany(x => x);
        }

        private IEnumerable<IList<Type>> selectGroups(TypeClassification classification)
        {
            return selectShelves(classification).SelectMany(x => x.SelectLists(classification));
        }

        private IEnumerable<AssemblyShelf> selectShelves(TypeClassification classification)
        {
            var open = classification.HasFlag(TypeClassification.Open);
            var closed = classification.HasFlag(TypeClassification.Closed);

            if ((open && closed) || (!open && !closed))
            {
                yield return OpenTypes;
                yield return ClosedTypes;
            }
            else if (open)
            {
                yield return OpenTypes;
            }
            else if (closed)
            {
                yield return ClosedTypes;
            }
        }
    }

    public class AssemblyShelf
    {
        public readonly IList<Type> Interfaces = new List<Type>();
        public readonly IList<Type> Concretes = new List<Type>();
        public readonly IList<Type> Abstracts = new List<Type>();

        public IEnumerable<IList<Type>> SelectLists(TypeClassification classification)
        {
            var interfaces = classification.HasFlag(TypeClassification.Interfaces);
            var concretes = classification.HasFlag(TypeClassification.Concretes);
            var abstracts = classification.HasFlag(TypeClassification.Abstracts);

            if (interfaces || concretes || abstracts)
            {
                if (interfaces) yield return Interfaces;
                if (concretes) yield return Concretes;
                if (abstracts) yield return Abstracts;
            }
            else
            {
                yield return Interfaces;
                yield return Concretes;
                yield return Abstracts;
            }
        }

        public IEnumerable<Type> AllTypes()
        {
            return Interfaces.Concat(Concretes).Concat(Abstracts);
        }

        public void Add(Type type)
        {
            if (type.IsInterface)
            {
                Interfaces.Add(type);
            }
            else if (type.IsAbstract)
            {
                Abstracts.Add(type);
            }
            else if (type.IsClass)
            {
                Concretes.Add(type);
            }
        }
    }
}