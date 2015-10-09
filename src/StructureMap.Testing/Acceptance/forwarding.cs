using NUnit.Framework;
using Shouldly;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class forwarding
    {
        // SAMPLE: forwarding-sample-types
        public interface IWriter { }
        public interface IReader { }

        public class StatefulCache : IReader, IWriter
        {
            
        }
        // ENDSAMPLE

        // SAMPLE: forwarding-in-action
        [Test]
        public void stateful_cache_serves_multiple_interfaces()
        {
            var container = new Container(_ =>
            {
                // Let's make StatefulCache a SingletonThing in the container
                _.ForConcreteType<StatefulCache>().Configure.Singleton();

                _.Forward<StatefulCache, IReader>();
                _.Forward<StatefulCache, IWriter>();
            });


            container.GetInstance<IReader>().ShouldBeOfType<StatefulCache>();
            container.GetInstance<IWriter>().ShouldBeOfType<StatefulCache>();
        }
        // ENDSAMPLE

        // SAMPLE: forward-without-forward
        [Test]
        public void equivalent()
        {
            var container = new Container(_ =>
            {
                // Let's make StatefulCache a SingletonThing in the container
                _.ForConcreteType<StatefulCache>().Configure.Singleton();

                _.For<IReader>().Use(c => c.GetInstance<StatefulCache>());
                _.For<IWriter>().Use(c => c.GetInstance<StatefulCache>());
            });

            container.GetInstance<IReader>().ShouldBeOfType<StatefulCache>();
            container.GetInstance<IWriter>().ShouldBeOfType<StatefulCache>();
        }
        // ENDSAMPLE
    }
}