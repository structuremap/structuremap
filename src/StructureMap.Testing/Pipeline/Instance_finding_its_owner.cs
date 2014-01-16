using System;
using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class Instance_finding_its_owner
    {
        [Test]
        public void nothing_is_attached()
        {
            new SimpleInstance().Owner().ShouldBeNull();
        }

        [Test]
        public void has_family_but_family_does_not_have_owner()
        {
            var family = new PluginFamily(GetType());
            var instance = new SimpleInstance();
            family.AddInstance(instance);

            instance.Owner().ShouldBeNull();
        }

        [Test]
        public void has_family_and_family_has_parent()
        {
            var graph = new PluginGraph();

            var family = graph.Families[GetType()];
            var instance = new SimpleInstance();
            family.AddInstance(instance);

            instance.Owner().ShouldBeTheSameAs(graph);
        }

        [Test]
        public void get_the_owner_when_part_of_a_profile()
        {
            var graph = new PluginGraph();
            var profile = graph.Profile("something");

            var family = profile.Families[GetType()];
            var instance = new SimpleInstance();
            family.AddInstance(instance);

            instance.Owner().ShouldBeTheSameAs(graph);
        }


        [Test]
        public void get_the_owner_when_part_of_a_deep_profile()
        {
            var graph = new PluginGraph();
            var profile = graph.Profile("something").Profile("else").Profile("again");

            var family = profile.Families[GetType()];
            var instance = new SimpleInstance();
            family.AddInstance(instance);

            instance.Owner().ShouldBeTheSameAs(graph);
        }
    }

    public class SimpleInstance : Instance
    {
        public override IDependencySource ToDependencySource(Type pluginType)
        {
            throw new System.NotImplementedException();
        }

        public override Type ReturnedType
        {
            get { return null; }
        }

        protected override string getDescription()
        {
            return "simple";
        }
    }
}