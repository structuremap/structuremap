using System.Linq;
using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class GetAllInstancesShouldObserveKeys
    {
        interface IService { }
        class Service : IService {}
        class Thingy
        {
            public readonly IService Service;

            public Thingy(IService service)
            {
                Service = service;
            }
        }

        // TODO -- go find this in GH.  I'm saying that we won't do this.
        [Test, Ignore("bug #44: not fixed yet")]
        public void It_should_observe_keys()
        {
            using (var container = new Container(cfg =>
                                              {
                                                  cfg.For<IService>().Use<Service>();
                                                  cfg.For<Thingy>().Use<Thingy>();
                                              }))
            {
                var service = new Service();
                var thingy = container.With("service").EqualTo(service).GetAllInstances<Thingy>().First();
                thingy.Service.ShouldBeTheSameAs(service);
            }
        }
    }
}
