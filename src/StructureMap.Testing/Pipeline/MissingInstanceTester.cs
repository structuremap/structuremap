using System;
using NUnit.Framework;
using Shouldly;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class MissingInstanceTester
    {
        // SAMPLE: missing-instance-objects
        public interface Rule
        {
        }

        public class ColorRule : Rule
        {
            public string Color { get; set; }

            public ColorRule(string color)
            {
                Color = color;
            }
        }

        // ENDSAMPLE

        // SAMPLE: missing-instance-simple-usage
        [Test]
        public void configure_and_use_missing_instance()
        {
            var container = new Container(x =>
            {
                x.For<Rule>().MissingNamedInstanceIs
                    .ConstructedBy(context => new ColorRule(context.RequestedName));
            });

            container.GetInstance<Rule>("red")
                .ShouldBeOfType<ColorRule>().Color.ShouldBe("red");

            container.GetInstance<Rule>("green")
                .ShouldBeOfType<ColorRule>().Color.ShouldBe("green");

            container.GetInstance<Rule>("blue")
                .ShouldBeOfType<ColorRule>().Color.ShouldBe("blue");
        }

        // ENDSAMPLE

        // SAMPLE: missing-instance-does-not-override-explicit
        [Test]
        public void does_not_override_explicit_registrations()
        {
            var container = new Container(x =>
            {
                x.For<Rule>().Add(new ColorRule("DarkRed")).Named("red");

                x.For<Rule>().MissingNamedInstanceIs
                    .ConstructedBy(context => new ColorRule(context.RequestedName));
            });

            container.GetInstance<Rule>("red")
                .ShouldBeOfType<ColorRule>()
                .Color.ShouldBe("DarkRed");
        }

        // ENDSAMPLE

        // SAMPLE: missing-instance-with-Instance-registration
        [Test]
        public void configure_and_use_missing_instance_by_generic_registration()
        {
            var instance = new LambdaInstance<ColorRule>(c => new ColorRule(c.RequestedName));

            var container = new Container(x =>
            {
                x.For(typeof (Rule))
                    .MissingNamedInstanceIs(instance);
            });

            container.GetInstance<Rule>("red").ShouldBeOfType<ColorRule>().Color.ShouldBe("red");
            container.GetInstance<Rule>("green").ShouldBeOfType<ColorRule>().Color.ShouldBe("green");
            container.GetInstance<Rule>("blue").ShouldBeOfType<ColorRule>().Color.ShouldBe("blue");
        }

        // ENDSAMPLE

        [Test]
        public void returns_missing_instance_if_it_exists_and_the_requested_instance_is_not_found()
        {
            var graph = new PluginGraph();
            var family = graph.Families[typeof (IWidget)];
            var missing = new ObjectInstance(new AWidget());
            family.MissingInstance = missing;

            var missingInstance = graph.FindInstance(typeof (IWidget), "anything") as Instance.MissingInstance;
			missingInstance.ShouldNotBeNull();
			missingInstance.InnerInstance.ShouldBeTheSameAs(missing);

            missingInstance = graph.FindInstance(typeof (IWidget), Guid.NewGuid().ToString()) as Instance.MissingInstance;
			missingInstance.ShouldNotBeNull();
			missingInstance.InnerInstance.ShouldBeTheSameAs(missing);
        }
    }
}