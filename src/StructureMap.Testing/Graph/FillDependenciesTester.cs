using Shouldly;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget4;
using Xunit;

namespace StructureMap.Testing.Graph
{
    public class FillDependenciesTester
    {
        [Fact]
        public void CanFillDependenciesSuccessfully()
        {
            var container = new Container(x =>
            {
                x.For<IStrategy>().Use(new Strategy("name", 3, 3, 3, true));
                x.For<IWidget>().Use(new ColorWidget("Red"));
            });

            var concreteClass =
                (FilledConcreteClass)container.GetInstance(typeof(FilledConcreteClass));

            concreteClass.Widget.ShouldNotBeNull();
            concreteClass.Strategy.ShouldNotBeNull();
        }

        [Fact]
        public void TryToFillDependenciesOnAbstractClassThrowsException()
        {
            var ex = Exception<StructureMapConfigurationException>.ShouldBeThrownBy(() =>
            {
                var container = new Container();
                container.GetInstance(typeof(AbstractClass));
            });

            ex.Title.ShouldBe(
                "No default Instance is registered and cannot be automatically determined for type 'StructureMap.Testing.Graph.AbstractClass'");
        }

        [Fact]
        public void TryToFillDependenciesOnClassWithPrimitiveArgumentsThrowsException()
        {
            Exception<StructureMapBuildPlanException>.ShouldBeThrownBy(() =>
            {
                var container = new Container();
                container.GetInstance(typeof(CannotBeFilledConcreteClass));
            });
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