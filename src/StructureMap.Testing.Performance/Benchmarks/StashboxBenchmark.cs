using Stashbox;
using System;

namespace StructureMap.Testing.Performance.Benchmarks
{
    public class StashboxBenchmark : BaseBenchmark
    {
        private readonly StashboxContainer _container;

        public StashboxBenchmark()
        {
            _container = new StashboxContainer();
            foreach (var map in TransientMap)
            {
                _container.RegisterType(map.Item1, map.Item2);
            }

            foreach (var map in SingletonMap)
            {
                _container.RegisterType(map.Item1, map.Item2);
            }
        }

        protected override void Resolve(Type type)
        {
            _container.Resolve(type);
        }
    }
}