using System;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class MissingInstanceTester
    {
        [Test]
        public void configure_and_use_missing_instance()
        {
            // If a user should happen to ask for a Rule by name
            // that does not exist, StructureMap will use an Instance
            // that builds a "ColorRule" object using the 
            // IContext.RequestedName property
            var container = new Container(x => {
                x.For<Rule>().MissingNamedInstanceIs
                    .ConstructedBy(context => new ColorRule(context.RequestedName));
            });

            container.GetInstance<Rule>("red").ShouldBeOfType<ColorRule>().Color.ShouldEqual("red");
            container.GetInstance<Rule>("green").ShouldBeOfType<ColorRule>().Color.ShouldEqual("green");
            container.GetInstance<Rule>("blue").ShouldBeOfType<ColorRule>().Color.ShouldEqual("blue");
        }

        [Test]
        public void returns_missing_instance_if_it_exists_and_the_requested_instance_is_not_found()
        {
            var graph = new PluginGraph();
            var family = graph.Families[typeof (IWidget)];
            var missing = new ObjectInstance(new AWidget());
            family.MissingInstance = missing;

            graph.FindInstance(typeof (IWidget), "anything").ShouldBeTheSameAs(missing);
            graph.FindInstance(typeof (IWidget), Guid.NewGuid().ToString()).ShouldBeTheSameAs(missing);
        }
    }
}