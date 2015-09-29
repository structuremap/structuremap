using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using StructureMap.Util;

namespace StructureMap.Graph.Scanning
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
}