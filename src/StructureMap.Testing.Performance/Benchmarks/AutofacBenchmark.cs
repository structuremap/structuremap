using Autofac;
using System;

namespace StructureMap.Testing.Performance.Benchmarks
{
    public class AutofacBenchmark : BaseBenchmark
    {
        private readonly Autofac.IContainer _container;

        public AutofacBenchmark()
        {
            var builder = new ContainerBuilder();

            foreach (var map in TransientMap)
            {
                builder.RegisterType(map.Item2).As(map.Item1);
            }

            foreach (var map in SingletonMap)
            {
                builder.RegisterType(map.Item2).As(map.Item1).SingleInstance();
            }
            _container = builder.Build();
        }

        protected override void Resolve(Type type)
        {
            _container.Resolve(type);
        }
    }
}