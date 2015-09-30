using System;
using System.Collections.Generic;
using System.Linq;

namespace StructureMap.Graph.Scanning
{
    public class TypeSet
    {
        private readonly IEnumerable<AssemblyTypes> _allTypes;
        private readonly Func<Type, bool> _filter = type => true;

        public TypeSet(IEnumerable<AssemblyTypes> allTypes, Func<Type, bool> filter = null)
        {
            _allTypes = allTypes;
            _filter = filter ?? _filter;
        }

        public IEnumerable<AssemblyScanRecord> Records
        {
            get { return _allTypes.Select(x => x.Record); }
        }

        public IEnumerable<Type> FindTypes(TypeClassification classification)
        {
            return _allTypes.SelectMany(x => x.FindTypes(classification)).Where(_filter).ToArray();
        }

        public IEnumerable<Type> AllTypes()
        {
            return _allTypes.SelectMany(x => x.FindTypes(TypeClassification.All)).Where(_filter).ToArray();
        }
    }
}