using NUnit.Framework;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class RedirectingTester
    {
        [Test]
        public void can_successfully_redirect()
        {
            var container = new Container(x =>
            {
                x.For<IOne>().Use<OneAndTwo>();
                x.Forward<IOne, ITwo>();
            });

            container.GetInstance<IOne>().ShouldBeOfType<OneAndTwo>();
        }
    }

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