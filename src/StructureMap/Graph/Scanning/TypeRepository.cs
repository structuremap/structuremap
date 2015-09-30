using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StructureMap.Graph.Scanning
{
    // Really only tested in integration with other things
    public static class TypeRepository
    {
        private static readonly ConcurrentDictionary<Assembly, Task<AssemblyTypes>> _assemblies = new ConcurrentDictionary<Assembly, Task<AssemblyTypes>>();

        public static void ClearAll()
        {
            _assemblies.Clear();
        }

        /// <summary>
        /// Use to assert that there were no failures in type scanning when trying to find the exported types
        /// from any Assembly
        /// </summary>
        public static void AssertNoTypeScanningFailures()
        {
            var exceptions =
                FailedAssemblies().Select(x => x.Record.LoadException);


            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }

        public static IEnumerable<AssemblyTypes> FailedAssemblies()
        {
            var tasks = _assemblies.Select(x => x.Value).ToArray();
            Task.WaitAll(tasks);

            return tasks.Where(x => x.Result.Record.LoadException != null).Select(x => x.Result);
        }

        public static Task<AssemblyTypes> ForAssembly(Assembly assembly)
        {
            return _assemblies.GetOrAdd(assembly, assem => Task.Factory.StartNew(() => new AssemblyTypes(assem)));
        }

        public static Task<TypeSet> FindTypes(IEnumerable<Assembly> assemblies, Func<Type, bool> filter = null)
        {
            var tasks = assemblies.Select(x => ForAssembly(x)).ToArray();
            return Task.Factory.ContinueWhenAll(tasks, assems =>
            {
                return new TypeSet(assems.Select(x => x.Result).ToArray(), filter);
            });
        }


        public static Task<IEnumerable<Type>> FindTypes(IEnumerable<Assembly> assemblies,
            TypeClassification classification, Func<Type, bool> filter = null)
        {
            var query = new TypeQuery(classification, filter);

            var tasks = assemblies.Select(assem => ForAssembly(assem).ContinueWith(t => query.Find(t.Result))).ToArray();
            return Task.Factory.ContinueWhenAll(tasks, results => results.SelectMany(x => x.Result));
        }

        public static Task<IEnumerable<Type>> FindTypes(Assembly assembly, TypeClassification classification,
            Func<Type, bool> filter = null)
        {
            var query = new TypeQuery(classification, filter);

            return ForAssembly(assembly).ContinueWith(t => query.Find(t.Result));
        }
    }
}