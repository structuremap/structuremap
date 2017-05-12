using Grace.DependencyInjection;
using System;

namespace StructureMap.Testing.Performance.Benchmarks
{
    public class GraceBenchmark : BaseBenchmark
    {
        private readonly DependencyInjectionContainer _container;

        public GraceBenchmark()
        {
            _container = new DependencyInjectionContainer();
            foreach (var map in TransientMap)
            {
                _container.Configure(c => c.Export(map.Item2).As(map.Item1));
            }

            foreach (var map in SingletonMap)
            {
                _container.Configure(c => c.Export(map.Item2).As(map.Item1).Lifestyle.Singleton());
            }
        }

        protected override void Resolve(Type type)
        {
            _container.Locate(type);
        }
    }
}