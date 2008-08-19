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

            IContainer manager = new Container(registry => registry.ForRequestedType<Abstraction>().TheDefaultIs(
                                                               ConstructedBy<Abstraction>(() => concretion)
                                                               ));

            Assert.AreSame(concretion, manager.GetInstance<Abstraction>());
        }

        [Test]
        public void ConstructSomethingByName()
        {
            Concretion concretion1 = new Concretion();
            Concretion concretion2 = new Concretion();

            IContainer manager = new Container(registry =>
            {
                registry.ForRequestedType<Abstraction>().AddInstance(
                    ConstructedBy<Abstraction>(() => concretion1).WithName("One")
                    );

                registry.ForRequestedType<Abstraction>().AddInstance(
                    ConstructedBy<Abstraction>(() => concretion2).WithName("Two")
                    );
            });

            Assert.AreSame(concretion1, manager.GetInstance<Abstraction>("One"));
            Assert.AreSame(concretion2, manager.GetInstance<Abstraction>("Two"));
        }

        [Test]
        public void ConstructSomethingNotByDefault()
        {
            Concretion concretion = new Concretion();

            IContainer manager = new Container(registry => registry.ForRequestedType<Abstraction>().AddInstance(
                                                               ConstructedBy<Abstraction>(() => concretion)
                                                               ));

            Abstraction actual = manager.GetAllInstances<Abstraction>()[0];
            Assert.AreSame(concretion, actual);
        }
    }
}