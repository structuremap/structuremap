extern alias SM3;

using System;

namespace StructureMap.Testing.Performance.Benchmarks
{
    public class StructureMap3Benchmark : BaseBenchmark
    {
        private readonly SM3::StructureMap.Testing.Performance.SM3Last.SM3Container _container;

        public StructureMap3Benchmark()
        {
            _container = new SM3::StructureMap.Testing.Performance.SM3Last.SM3Container(TransientMap, SingletonMap);
        }

        protected override void Resolve(Type type)
        {
            _container.GetInstance(type);
        }
    }
}