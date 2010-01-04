using NUnit.Framework;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget4;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class FillDependenciesTester
    {
        [Test]
        public void CanFillDependenciesSuccessfully()
        {
            var container = new Container(x =>
            {
                x.For<IStrategy>().TheDefault.IsThis(new Strategy("name", 3, 3, 3, true));
                x.For<IWidget>().TheDefault.IsThis(new ColorWidget("Red"));
            });

            var concreteClass =
                (FilledConcreteClass) container.GetInstance(typeof (FilledConcreteClass));

            Assert.IsNotNull(concreteClass.Widget);
            Assert.IsNotNull(concreteClass.Strategy);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void TryToFillDependenciesOnAbstractClassThrowsException()
        {
            var manager = new Container();
            manager.GetInstance(typeof (AbstractClass));
        }


        [Test, ExpectedException(typeof (StructureMapException))]
        public void TryToFillDependenciesOnClassWithPrimitiveArgumentsThrowsException()
        {
            var manager = new Container();
            manager.GetInstance(typeof (CannotBeFilledConcreteClass));
        }
    }

    public class FilledConcreteClass
    {
        private readonly IStrategy _strategy;
        private readonly IWidget _widget;

        public FilledConcreteClass(IStrategy strategy, IWidget widget)
        {
            _strategy = strategy;
            _widget = widget;
        }

        public IStrategy Strategy { get { return _strategy; } }

        public IWidget Widget { get { return _widget; } }
    }

    public class CannotBeFilledConcreteClass
    {
        public CannotBeFilledConcreteClass(string name, Rule rule)
        {
        }
    }

    public abstract class AbstractClass
    {
    }
}