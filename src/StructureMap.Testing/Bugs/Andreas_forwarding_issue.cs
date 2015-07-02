using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Andreas_forwarding_issue
    {
        [Test]
        public void can_do_the_forwarding()
        {
            var container = new Container(_ =>
            {
                _.ForSingletonOf<ConcreteGuy>().Use<ConcreteGuy>();
                _.For<IGuy<string>>().Use(c => c.GetInstance<ConcreteGuy>());
            });

            var guy = container.GetInstance<ConcreteGuy>();
            guy.ShouldBeTheSameAs(container.GetInstance<IGuy<string>>());
        }
    }

    public interface IGuy<T>
    {
    }

    public class ConcreteGuy : IGuy<string>
    {
    }
}