using System;
using System.Collections.Generic;
using System.Linq;

namespace StructureMap.Graph.Scanning
{
    /// <summary>
    /// Access to a set of exported .Net Type's as defined in a scanning operation
    /// </summary>
    public class TypeSet
    {
        private readonly IEnumerable<AssemblyTypes> _allTypes;
        private readonly Func<Type, bool> _filter = type => true;

        public TypeSet(IEnumerable<AssemblyTypes> allTypes, Func<Type, bool> filter = null)
        {
            _allTypes = allTypes;
            _filter = filter ?? _filter;
        }

        /// <summary>
        /// For diagnostic purposes, explains which assemblies were
        /// scanned as part of this TypeSet, including failures
        /// </summary>
        public IEnumerable<AssemblyScanRecord> Records
        {
            get { return _allTypes.Select(x => x.Record); }
        }

        /// <summary>
        /// Find any types in this TypeSet that match any combination of the TypeClassification enumeration values
        /// </summary>
        /// <param name="classification"></param>
        /// <returns></returns>
        public IEnumerable<Type> FindTypes(TypeClassification classification)
        {
            return _allTypes.SelectMany(x => x.FindTypes(classification)).Where(_filter).ToArray();
        }

        /// <summary>
        /// Returns all the types in this TypeSet
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> AllTypes()
        {
            return _allTypes.SelectMany(x => x.FindTypes(TypeClassification.All)).Where(_filter).ToArray();
        }
    }
}