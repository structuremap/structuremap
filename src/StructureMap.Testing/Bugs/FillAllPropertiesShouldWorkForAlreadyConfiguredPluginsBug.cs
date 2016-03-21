using System.Collections.Generic;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class FillAllPropertiesShouldWorkForAlreadyConfiguredPluginsBug
    {
        [Fact]
        public void Structuremap_should_autowire_setter_dependencies_regardless_of_order()
        {
            var container = new Container();

            container.Configure(x => { x.ForConcreteType<ClassWithSetterDependency>(); });

            container.Configure(x =>
            {
                x.For<ISomeDependency>()
                    .Use<ClassThatImplementsDependency>();

                x.Policies.FillAllPropertiesOfType<ISomeDependency>();
            });

            container.GetInstance<ClassWithSetterDependency>().Dependency.ShouldNotBeNull();
        }
    }

    public class ClassWithSetterDependency
    {
        public ISomeDependency Dependency { get; set; }
        public IList<string> SystemDependency { get; set; }
    }

    public interface ISomeDependency
    {
    }

    public class ClassThatImplementsDependency : ISomeDependency
    {
    }

    public interface INonConfiguredInterface
    {
    }
}