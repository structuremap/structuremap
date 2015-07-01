using NUnit.Framework;
using Shouldly;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class RedirectingTester
    {
        [Test]
        public void can_successfully_redirect()
        {
            var container = new Container(x => {
                x.For<IOne>().Use<OneAndTwo>();
                x.Forward<IOne, ITwo>();
            });

            container.GetInstance<IOne>().ShouldBeOfType<OneAndTwo>();
        }

        [Test]
        public void can_redirect_with_a_singleton()
        {
            var container = new Container(_ =>
            {
                _.ForSingletonOf<IBase>().Use<Service>();
                _.Forward<IBase, IDerived>();
            });

            container.GetInstance<IBase>().ShouldBeOfType<Service>();
            container.GetInstance<IDerived>().ShouldBeOfType<Service>();

            container.GetInstance<IBase>().ShouldBeTheSameAs(container.GetInstance<IDerived>());
        }
    }

    interface IBase { }
    interface IDerived : IBase { }
    class Service : IDerived { }

    public interface IOne
    {
    }

    public interface ITwo
    {
    }

    public class OneAndTwo : IOne, ITwo
    {
    }
}