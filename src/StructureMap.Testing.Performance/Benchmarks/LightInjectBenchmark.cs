using LightInject;
using System;

namespace StructureMap.Testing.Performance.Benchmarks
{
    public class LightInjectBenchmark : BaseBenchmark
    {
        private readonly ServiceContainer _container;

        public LightInjectBenchmark()
        {
            _container = new ServiceContainer();
            foreach (var map in TransientMap)
            {
                _container.Register(map.Item1, map.Item2);
            }

            foreach (var map in SingletonMap)
            {
                _container.Register(map.Item1, map.Item2, new PerContainerLifetime());
            }
        }

        protected override void Resolve(Type type)
        {
            _container.GetInstance(type);
        }
    }
}