using System.Collections.Generic;
using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class FillAllPropertiesShouldWorkForAlreadyConfiguredPluginsBug
    {
        [Test]
        public void Structuremap_should_autowire_setter_dependencies_regardless_of_order()
        {
            var container = new Container();

            container.Configure(x => { x.ForConcreteType<ClassWithSetterDependency>(); });


            container.Configure(x =>
            {
                x.For<ISomeDependency>()
                    .TheDefaultIsConcreteType<ClassThatImplementsDependency>();

                x.FillAllPropertiesOfType<ISomeDependency>();
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