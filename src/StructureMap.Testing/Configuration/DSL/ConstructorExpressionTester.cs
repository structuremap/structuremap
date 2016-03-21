using System.Linq;
using Xunit;

namespace StructureMap.Testing.Configuration.DSL
{
    public class ConstructorExpressionTester : Registry
    {
        public interface Abstraction
        {
        }

        public class Concretion : Abstraction
        {
        }

        [Fact]
        public void AddTwoConstructorsConsecutively()
        {
            var concretion1 = new Concretion();
            var concretion2 = new Concretion();

            IContainer container = new Container(r =>
                r.For<Abstraction>().AddInstances(x =>
                {
                    x.ConstructedBy(() => concretion1).Named("One");
                    x.ConstructedBy(() => concretion2).Named("Two");
                }));

            concretion1.ShouldBeTheSameAs(container.GetInstance<Abstraction>("One"));
            concretion2.ShouldBeTheSameAs(container.GetInstance<Abstraction>("Two"));
        }

        [Fact]
        public void ConstructSomething()
        {
            var concretion = new Concretion();

            IContainer container =
                new Container(
                    registry => registry.For<Abstraction>().Use(() => concretion));
            container.GetInstance<Abstraction>().ShouldBeTheSameAs(concretion);
        }

        [Fact]
        public void ConstructSomethingByName()
        {
            var concretion1 = new Concretion();
            var concretion2 = new Concretion();

            IContainer manager = new Container(registry =>
            {
                registry.For<Abstraction>().AddInstances(x =>
                {
                    x.ConstructedBy(() => concretion1).Named("One");
                    x.ConstructedBy(() => concretion2).Named("Two");
                });
            });

            manager.GetInstance<Abstraction>("One").ShouldBeTheSameAs(concretion1);
            manager.GetInstance<Abstraction>("Two").ShouldBeTheSameAs(concretion2);
        }

        [Fact]
        public void ConstructSomethingNotByDefault()
        {
            var concretion = new Concretion();

            var container = new Container(r => { r.For<Abstraction>().Add(c => concretion); });

            container.GetAllInstances<Abstraction>().First().ShouldBeTheSameAs(concretion);
        }
    }
}