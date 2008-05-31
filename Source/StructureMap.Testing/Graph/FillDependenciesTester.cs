using NUnit.Framework;
using StructureMap.Graph;
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
            PluginGraph pluginGraph = ObjectMother.GetPluginGraph();

            Container manager = new Container(pluginGraph);

            // The dependencies must have a default setting first
            manager.SetDefault(typeof (IStrategy), "Red");
            manager.SetDefault(typeof (IWidget), "Blue");
            IWidget widget = (IWidget) manager.GetInstance(typeof (IWidget));
            IStrategy strategy = (IStrategy) manager.GetInstance(typeof (IStrategy));

            FilledConcreteClass concreteClass =
                (FilledConcreteClass) manager.FillDependencies(typeof (FilledConcreteClass));

            Assert.IsNotNull(concreteClass.Widget);
            Assert.IsNotNull(concreteClass.Strategy);
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void TryToFillDependenciesOnAbstractClassThrowsException()
        {
            PluginGraph pluginGraph = ObjectMother.GetPluginGraph();
            Container manager = new Container(pluginGraph);

            manager.FillDependencies(typeof (AbstractClass));
        }


        [Test, ExpectedException(typeof (StructureMapException))]
        public void TryToFillDependenciesOnClassWithPrimitiveArgumentsThrowsException()
        {
            PluginGraph pluginGraph = ObjectMother.GetPluginGraph();
            Container manager = new Container(pluginGraph);

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