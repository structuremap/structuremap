using NUnit.Framework;
using StructureMap.Configuration.DSL;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class ConstructorExpressionTester : Registry
    {
        public interface Abstraction
        {
        }

        public class Concretion : Abstraction
        {
        }

        [Test]
        public void AddTwoConstructorsConsecutively()
        {
            Concretion concretion1 = new Concretion();
            Concretion concretion2 = new Concretion();

            IContainer container = new Container(r => 
                r.ForRequestedType<Abstraction>().AddInstances(x =>
                {
                    x.ConstructedBy(() => concretion1).WithName("One");
                    x.ConstructedBy(() => concretion2).WithName("Two");
                }));

            Assert.AreSame(concretion1, container.GetInstance<Abstraction>("One"));
            Assert.AreSame(concretion2, container.GetInstance<Abstraction>("Two"));
        }

        [Test]
        public void ConstructSomething()
        {
            Concretion concretion = new Concretion();

            IContainer container = new Container(registry => registry.ForRequestedType<Abstraction>().TheDefault.Is.ConstructedBy(() => concretion));
            container.GetInstance<Abstraction>().ShouldBeTheSameAs(concretion);
        }

        [Test]
        public void ConstructSomethingByName()
        {
            Concretion concretion1 = new Concretion();
            Concretion concretion2 = new Concretion();

            IContainer manager = new Container(registry =>
            {
                registry.ForRequestedType<Abstraction>().AddInstances(x =>
                {
                    x.ConstructedBy(() => concretion1).WithName("One");
                    x.ConstructedBy(() => concretion2).WithName("Two");
                });

            });

            manager.GetInstance<Abstraction>("One").ShouldBeTheSameAs(concretion1);
            manager.GetInstance<Abstraction>("Two").ShouldBeTheSameAs(concretion2);
        }

        [Test]
        public void ConstructSomethingNotByDefault()
        {
            Concretion concretion = new Concretion();

            var container = new Container(r =>
            {
                r.InstanceOf<Abstraction>().Is.ConstructedBy(() => concretion);
            });

            Abstraction actual = container.GetAllInstances<Abstraction>()[0];
            Assert.AreSame(concretion, actual);
        }
    }
}