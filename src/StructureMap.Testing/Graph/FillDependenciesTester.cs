using System.Diagnostics;
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
            var container = new Container(x => {
                x.For<IStrategy>().Use(new Strategy("name", 3, 3, 3, true));
                x.For<IWidget>().Use(new ColorWidget("Red"));
            });

            var concreteClass =
                (FilledConcreteClass) container.GetInstance(typeof (FilledConcreteClass));

            Assert.IsNotNull(concreteClass.Widget);
            Assert.IsNotNull(concreteClass.Strategy);
        }

        [Test]
        public void TryToFillDependenciesOnAbstractClassThrowsException()
        {
            var ex = Exception<StructureMapException>.ShouldBeThrownBy(() => {
                var container = new Container();
                container.GetInstance(typeof(AbstractClass));
            });

            ex.Title.ShouldBe("No default Instance is registered and cannot be automatically determined for type 'StructureMap.Testing.Graph.AbstractClass'");
        }


        [Test]
        public void TryToFillDependenciesOnClassWithPrimitiveArgumentsThrowsException()
        {
            var ex = Exception<StructureMapException>.ShouldBeThrownBy(() => {
                var container = new Container();
                container.GetInstance(typeof(CannotBeFilledConcreteClass));
            });

            ex.Title.ShouldBe("No default Instance is registered and cannot be automatically determined for type 'StructureMap.Testing.Graph.CannotBeFilledConcreteClass'");

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