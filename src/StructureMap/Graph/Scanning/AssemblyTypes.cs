using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.TypeRules;

namespace StructureMap.Graph.Scanning
{
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
}