using NUnit.Framework;
using StructureMap.Testing.Diagnostics;

namespace StructureMap.Testing.Examples
{
    [TestFixture]
    public class UsingContainerModel
    {
        [Test]
        public void finding_things()
        {
            var container = Container.For<VisualizationRegistry>();


            container.Model.PluginTypes.Each(x =>
            {

            });
        } 
    }
}