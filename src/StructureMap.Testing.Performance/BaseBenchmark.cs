using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StructureMap.Testing.Performance
{
    public abstract class BaseBenchmark
    {
        protected const int IterationsPerInvoke = 1000;

        protected BaseBenchmark()
        {
            var count = TypeCatalog.TypesMap.Length / 2;
            TransientMap = new ArraySegment<Tuple<Type, Type>>(TypeCatalog.TypesMap, 0, count);
            SingletonMap = new ArraySegment<Tuple<Type, Type>>(TypeCatalog.TypesMap, count,
                TypeCatalog.TypesMap.Length - count);
        }

        protected ArraySegment<Tuple<Type, Type>> TransientMap { get; }

        protected ArraySegment<Tuple<Type, Type>> SingletonMap { get; }

        [Params(Lifetime.Singleton, Lifetime.Transient)]
        public Lifetime Lifetime { get; set; }

        [Params(1, 2)]
        public int ThreadsCount { get; set; }

        [Params(2, 10, 100)]
        public int ResolvedTypesCount { get; set; }

        [Benchmark(OperationsPerInvoke = IterationsPerInvoke)]
        public virtual void Resolve()
        {
            if (ThreadsCount > 1)
            {
                Parallel.ForEach(GetTypes(), new ParallelOptions { MaxDegreeOfParallelism = ThreadsCount }, Resolve);
            }
            else
            {
                foreach (var type in GetTypes())
                {
                    Resolve(type);
                }
            }
        }

        private IEnumerable<Type> GetTypes()
        {
            var map = Lifetime == Lifetime.Transient ? TransientMap : SingletonMap;

            for (var i = 0; i < IterationsPerInvoke; i++)
            {
                var type = map.Array[map.Offset + i % ResolvedTypesCount % map.Count].Item1;
                yield return type;
            }
        }

        protected abstract void Resolve(Type type);
    }
}