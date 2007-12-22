using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.Graph;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class ConstructorExpressionTester : Registry
    {
        [SetUp]
        public void SetUp()
        {
            StructureMapConfiguration.ResetAll();
            ObjectFactory.Reset();
        }

        public interface Abstraction
        {
        }

        public class Concretion : Abstraction
        {
        }

        [Test]
        public void ConstructSomething()
        {
            Concretion concretion = new Concretion();

            Registry registry = new Registry();
            registry.ForRequestedType<Abstraction>().TheDefaultIs(
                ConstructedBy<Abstraction>(delegate { return concretion; })
                );

            IInstanceManager manager = registry.BuildInstanceManager();
            Assert.AreSame(concretion, manager.CreateInstance<Abstraction>());
        }

        [Test]
        public void ConstructSomethingNotByDefault()
        {
            Concretion concretion = new Concretion();

            Registry registry = new Registry();
            registry.ForRequestedType<Abstraction>().AddInstance(
                ConstructedBy<Abstraction>(delegate { return concretion; })
                );


            IInstanceManager manager = registry.BuildInstanceManager();

            Abstraction actual = manager.GetAllInstances<Abstraction>()[0];
            Assert.AreSame(concretion, actual);
        }

        [Test]
        public void ConstructSomethingByName()
        {
            Concretion concretion1 = new Concretion();
            Concretion concretion2 = new Concretion();

            Registry registry = new Registry();
            registry.ForRequestedType<Abstraction>().AddInstance(
                ConstructedBy<Abstraction>(delegate { return concretion1; }).WithName("One")
                );

            registry.ForRequestedType<Abstraction>().AddInstance(
                ConstructedBy<Abstraction>(delegate { return concretion2; }).WithName("Two")
                );

            IInstanceManager manager = registry.BuildInstanceManager();

            Assert.AreSame(concretion1, manager.CreateInstance<Abstraction>("One"));
            Assert.AreSame(concretion2, manager.CreateInstance<Abstraction>("Two"));
        }

        [Test]
        public void AddTwoConstructorsConsecutively()
        {
            Concretion concretion1 = new Concretion();
            Concretion concretion2 = new Concretion();

            Registry registry = new Registry();
            registry.ForRequestedType<Abstraction>()
                .AddInstance(
                    ConstructedBy<Abstraction>(delegate { return concretion1; }).WithName("One")
                )
                .AddInstance(
                    ConstructedBy<Abstraction>(delegate { return concretion2; }).WithName("Two")
                );

            IInstanceManager manager = registry.BuildInstanceManager();

            Assert.AreSame(concretion1, manager.CreateInstance<Abstraction>("One"));
            Assert.AreSame(concretion2, manager.CreateInstance<Abstraction>("Two"));
        }
    }
}