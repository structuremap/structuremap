using FubuMVC.Core.Runtime;
using NUnit.Framework;
using Shouldly;
using StructureMap.Testing;

namespace FubuMVC.StructureMap3.Testing
{
    [TestFixture]
    public class IServiceFactory_Compliance
    {
        [Test]
        public void has_the_IServiceFactory_registered()
        {
            ContainerFacilitySource.New(x => { })
                                   .Get<IServiceFactory>().ShouldBeOfType<StructureMapContainerFacility>();
        }
    }
}