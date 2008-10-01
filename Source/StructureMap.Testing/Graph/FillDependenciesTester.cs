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
                x.ForRequestedType<IStrategy>().TheDefault.IsThis(new Strategy("name", 3, 3, 3, true));
                x.ForRequestedType<IWidget>().TheDefault.IsThis(new ColorWidget("Red"));
            });

            var concreteClass =
                (FilledConcreteClass) container.FillDependencies(typeof (FilledConcreteClass));

            Assert.IsNotNull(concreteClass.Widget);
            Assert.IsNotNull(concreteClass.Strategy);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void TryToFillDependenciesOnAbstractClassThrowsException()
        {
            var manager = new Container();
            manager.FillDependencies(typeof (AbstractClass));
        }


        [Test, ExpectedException(typeof (StructureMapException))]
        public void TryToFillDependenciesOnClassWithPrimitiveArgumentsThrowsException()
        {
            var manager = new Container();
            manager.FillDependencies(typeof (CannotBeFilledConcreteClass));
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

        public IStrategy Strategy
        {
            get { return _strategy; }
        }

        public IWidget Widget
        {
            get { return _widget; }
        }
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