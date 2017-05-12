extern alias SM4;

using BenchmarkDotNet.Attributes;
using System;

namespace StructureMap.Testing.Performance.Benchmarks
{
    public class StructureMap4Benchmark : BaseBenchmark
    {
        private readonly SM4::StructureMap.Testing.Performance.SM4Last.SM4Container _container;

        public StructureMap4Benchmark()
        {
            _container = new SM4::StructureMap.Testing.Performance.SM4Last.SM4Container(TransientMap, SingletonMap);
        }

        [Benchmark(Baseline = true, OperationsPerInvoke = IterationsPerInvoke)]
        public override void Resolve()
        {
            base.Resolve();
        }

        protected override void Resolve(Type type)
        {
            _container.GetInstance(type);
        }
    }
}