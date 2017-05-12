using DryIoc;
using System;

namespace StructureMap.Testing.Performance.Benchmarks
{
    public class DryIocBenchmark : BaseBenchmark
    {
        private readonly DryIoc.IContainer _container;

        public DryIocBenchmark()
        {
            _container = new DryIoc.Container();
            foreach (var map in TransientMap)
            {
                _container.Register(map.Item1, map.Item2);
            }

            foreach (var map in SingletonMap)
            {
                _container.Register(map.Item1, map.Item2, Reuse.Singleton);
            }
        }

        protected override void Resolve(Type type)
        {
            _container.Resolve(type);
        }
    }
}