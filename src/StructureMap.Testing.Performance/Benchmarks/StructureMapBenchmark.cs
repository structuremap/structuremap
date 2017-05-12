using System;

namespace StructureMap.Testing.Performance.Benchmarks
{
    public class StructureMapBenchmark : BaseBenchmark
    {
        private readonly IContainer _container;

        public StructureMapBenchmark()
        {
            _container = new Container(cfg =>
            {
                foreach (var map in TransientMap)
                {
                    cfg.For(map.Item1).Use(map.Item2);
                }

                foreach (var map in SingletonMap)
                {
                    cfg.For(map.Item1).Use(map.Item2).Singleton();
                }
            });
        }

        protected override void Resolve(Type type)
        {
            _container.GetInstance(type);
        }
    }
}