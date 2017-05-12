using BenchmarkDotNet.Running;
using System.Linq;
using System.Reflection;

namespace StructureMap.Testing.Performance
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var benchmarkTypes = Assembly.GetEntryAssembly().DefinedTypes
                .Where(t => t.IsSubclassOf(typeof(BaseBenchmark)));

            var benchmarks = benchmarkTypes.SelectMany(t => BenchmarkConverter.TypeToBenchmarks(t)).ToArray();
            BenchmarkRunner.Run(benchmarks, null);
        }
    }
}