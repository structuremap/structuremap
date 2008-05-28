using NUnit.Framework;
using StructureMap.Configuration.DSL;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class ConstructorExpressionTester : Registry
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            StructureMapConfiguration.ResetAll();
            ObjectFactory.Reset();
        }

        #endregion

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

            IContainer manager = new InstanceManager(delegate(Registry registry)
            {
                registry.ForRequestedType<Abstraction>()
                    .AddInstances(
                        ConstructedBy<Abstraction>(delegate { return concretion1; }).WithName("One"),
                        ConstructedBy<Abstraction>(delegate { return concretion2; }).WithName("Two")
                    );
            });

            Assert.AreSame(concretion1, manager.GetInstance<Abstraction>("One"));
            Assert.AreSame(concretion2, manager.GetInstance<Abstraction>("Two"));
        }

        [Test]
        public void ConstructSomething()
        {
            Concretion concretion = new Concretion();

            IContainer manager = new InstanceManager(delegate(Registry registry)
            {
                registry.ForRequestedType<Abstraction>().TheDefaultIs(
                    ConstructedBy<Abstraction>(delegate { return concretion; })
                    );
            });

            Assert.AreSame(concretion, manager.GetInstance<Abstraction>());
        }

        [Test]
        public void ConstructSomethingByName()
        {
            Concretion concretion1 = new Concretion();
            Concretion concretion2 = new Concretion();

            IContainer manager = new InstanceManager(delegate(Registry registry)
            {
                registry.ForRequestedType<Abstraction>().AddInstance(
                    ConstructedBy<Abstraction>(delegate { return concretion1; }).WithName("One")
                    );

                registry.ForRequestedType<Abstraction>().AddInstance(
                    ConstructedBy<Abstraction>(delegate { return concretion2; }).WithName("Two")
                    );
            });

            Assert.AreSame(concretion1, manager.GetInstance<Abstraction>("One"));
            Assert.AreSame(concretion2, manager.GetInstance<Abstraction>("Two"));
        }

        [Test]
        public void ConstructSomethingNotByDefault()
        {
            Concretion concretion = new Concretion();

            IContainer manager = new InstanceManager(delegate(Registry registry)
            {
                registry.ForRequestedType<Abstraction>().AddInstance(
                    ConstructedBy<Abstraction>(delegate { return concretion; })
                    );
            });

            Abstraction actual = manager.GetAllInstances<Abstraction>()[0];
            Assert.AreSame(concretion, actual);
        }
    }
}