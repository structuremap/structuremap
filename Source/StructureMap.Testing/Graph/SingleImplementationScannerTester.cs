using System;
using NUnit.Framework;
using StructureMap.Graph;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class SingleImplementationScannerTester
    {
        [Test]
        public void other()
        {
            typeof (Type).IsValueType.ShouldBeFalse();
        }

        [Test, Ignore("Come back to this")]
        public void registers_plugins_that_only_have_a_single_implementation()
        {
            var container = new Container(registry =>
            {
                registry.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.IncludeNamespaceContainingType<SingleImplementationScannerTester>();
                    x.With(new SingleImplementationScanner());
                });
            });

            container.GetInstance<IOnlyHaveASingleConcreteImplementation>()
                .ShouldBeOfType<MyNameIsNotConventionallyRelatedToMyInterface>();
        }
    }


    public interface IOnlyHaveASingleConcreteImplementation
    {
    }

    public class MyNameIsNotConventionallyRelatedToMyInterface : IOnlyHaveASingleConcreteImplementation
    {
    }
}