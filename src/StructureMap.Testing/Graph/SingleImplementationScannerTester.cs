using NUnit.Framework;
using Shouldly;
using StructureMap.Graph;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class SingleImplementationScannerTester
    {
        private Container _container;

        [SetUp]
        public void Setup()
        {
            _container = new Container(registry => registry.Scan(x => {
                x.TheCallingAssembly();
                x.IncludeNamespaceContainingType<SingleImplementationScannerTester>();
                x.SingleImplementationsOfInterface();
            }));
        }

        [Test]
        public void registers_plugins_that_only_have_a_single_implementation()
        {
            _container.GetInstance<IOnlyHaveASingleConcreteImplementation>()
                .ShouldBeOfType<MyNameIsNotConventionallyRelatedToMyInterface>();
        }

        [Test]
        public void should_not_automatically_register_plugins_that_have_multiple_implementations()
        {
            _container.TryGetInstance<IHaveMultipleConcreteImplementations>().ShouldBeNull();
        }

        [Test]
        public void can_configure_plugin_families_via_dsl()
        {
            var differentContainer = new Container(registry => registry.Scan(x => {
                x.TheCallingAssembly();
                x.IncludeNamespaceContainingType<SingleImplementationScannerTester>();
                x.SingleImplementationsOfInterface().OnAddedPluginTypes(t => t.Singleton());
            }));

            var firstInstance = differentContainer.GetInstance<IOnlyHaveASingleConcreteImplementation>();
            var secondInstance = differentContainer.GetInstance<IOnlyHaveASingleConcreteImplementation>();
            secondInstance.ShouldBeTheSameAs(firstInstance);
        }
    }


    public interface IOnlyHaveASingleConcreteImplementation
    {
    }

    public class MyNameIsNotConventionallyRelatedToMyInterface : IOnlyHaveASingleConcreteImplementation
    {
    }

    public interface IHaveMultipleConcreteImplementations
    {
    }

    public class FirstConcreteImplementation : IHaveMultipleConcreteImplementations
    {
    }

    public class SecondConcreteImplementation : IHaveMultipleConcreteImplementations
    {
    }
}