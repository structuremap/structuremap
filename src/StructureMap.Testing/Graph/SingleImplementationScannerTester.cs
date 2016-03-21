using Shouldly;
using StructureMap.Graph;
using Xunit;

namespace StructureMap.Testing.Graph
{
    public class SingleImplementationScannerTester
    {
        private readonly Container container;

        public SingleImplementationScannerTester()
        {
            container = new Container(registry => registry.Scan(x =>
            {
                x.TheCallingAssembly();
                x.IncludeNamespaceContainingType<SingleImplementationScannerTester>();
                x.SingleImplementationsOfInterface();
            }));
        }

        [Fact]
        public void registers_plugins_that_only_have_a_single_implementation()
        {
            container.GetInstance<IOnlyHaveASingleConcreteImplementation>()
                .ShouldBeOfType<MyNameIsNotConventionallyRelatedToMyInterface>();
        }

        [Fact]
        public void should_not_automatically_register_plugins_that_have_multiple_implementations()
        {
            container.TryGetInstance<IHaveMultipleConcreteImplementations>().ShouldBeNull();
        }

        [Fact]
        public void can_configure_plugin_families_via_dsl()
        {
            var differentContainer = new Container(registry => registry.Scan(x =>
            {
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