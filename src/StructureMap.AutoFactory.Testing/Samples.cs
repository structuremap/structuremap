using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Shouldly;

namespace StructureMap.AutoFactory.Testing
{
    public class Samples
    {
        // SAMPLE: simple-factory
        [Fact]
        public void Simple_factory_creation()
        {
            var container = new Container(cfg =>
            {
                cfg.For<IDummyService>().Use<DummyService>();
                cfg.For<ISimpleDummyFactory>().CreateFactory();
            });

            var factory = container.GetInstance<ISimpleDummyFactory>();

            var component = factory.CreateDummyService();

            component.ShouldNotBeNull();
            component.ShouldBeOfType<DummyService>();
        }

        //ENDSAMPLE
    }

    // SAMPLE: ISimpleDummyFactory
    public interface ISimpleDummyFactory
    {
        IDummyService CreateDummyService();
    }

    //ENDSAMPLE
}