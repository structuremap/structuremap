using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing
{
    public class InitializeRegistry : Registry
    {
        public InitializeRegistry()
        {
            For<IWidget>().Add<ColorWidget>()
                .Ctor<string>("color").Is("Green")
                .Named("Green");
        }
    }

    [TestFixture]
    public class ObjectFactoryInitializeTester
    {
        [Test]
        public void Add_a_registry_by_generic_signature()
        {
            ObjectFactory.Initialize(x =>
            {
                x.AddRegistry<InitializeRegistry>();
            });

            ObjectFactory.GetNamedInstance<IWidget>("Green").ShouldBeOfType<ColorWidget>().Color.ShouldEqual("Green");
        }

        [Test]
        public void StructureMap_functions_without_StructureMapconfig_file_in_the_default_mode()
        {
            DataMother.RemoveStructureMapConfig();

            ObjectFactory.Initialize(x => { });
        }

        
        [Test]
        public void TheDefaultContainerName_should_be_ObjectFactory_Guid()
        {
            
            ObjectFactory.Initialize(x =>
            {
            });

            ObjectFactory.Container.Name.ShouldStartWith("ObjectFactory-");
        }

    }
}