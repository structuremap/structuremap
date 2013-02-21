using System;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Exceptions;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class PluginGraphTester
    {
        [Test, ExpectedException(typeof (StructureMapConfigurationException))]
        public void AssertErrors_throws_StructureMapConfigurationException_if_there_is_an_error()
        {
            var graph = new PluginGraph();
            graph.Log.RegisterError(400, new ApplicationException("Bad!"));

            graph.Log.AssertFailures();
        }

        [Test]
        public void add_type_adds_an_instance_for_type_once_and_only_once()
        {
            var graph = new PluginGraph();

            graph.AddType(typeof (IThingy), typeof (BigThingy));

            PluginFamily family = graph.FindFamily(typeof (IThingy));
            family.Instances
                  .Single()
                  .ShouldBeOfType<ConstructorInstance>()
                  .PluggedType.ShouldEqual(typeof (BigThingy));

            graph.AddType(typeof (IThingy), typeof (BigThingy));

            family.Instances.Count().ShouldEqual(1);
        }

        [Test]
        public void all_instances_when_family_has_not_been_created()
        {
            var graph = new PluginGraph();
            graph.AllInstances(typeof (BigThingy)).Any().ShouldBeFalse();

            graph.Families.Has(typeof (BigThingy)).ShouldBeFalse();
        }

        [Test]
        public void all_instances_when_the_family_already_exists()
        {
            var graph = new PluginGraph();
            graph.Families.FillDefault(typeof (BigThingy));

            graph.AllInstances(typeof (BigThingy)).Any().ShouldBeFalse();
        }

        [Test]
        public void eject_family_removes_the_family_and_disposes_all_of_its_instances()
        {
            var instance1 = new FakeInstance();
            var instance2 = new FakeInstance();
            var instance3 = new FakeInstance();

            var graph = new PluginGraph();
            graph.Families[typeof(IThingy)].AddInstance(instance1);
            graph.Families[typeof(IThingy)].AddInstance(instance2);
            graph.Families[typeof(IThingy)].AddInstance(instance3);

            graph.EjectFamily(typeof(IThingy));

            instance1.WasDisposed.ShouldBeTrue();
            instance2.WasDisposed.ShouldBeTrue();
            instance3.WasDisposed.ShouldBeTrue();

            graph.Families.Has(typeof (IThingy));
        }

        [Test]
        public void find_family_by_closing_an_open_interface_that_matches()
        {
            PluginGraph graph = PluginGraph.Empty();
            graph.Families[typeof (IOpen<>)].SetDefault(new ConfiguredInstance(typeof (Open<>)));

            graph.Families[typeof (IOpen<string>)].GetDefaultInstance().ShouldBeOfType<ConstructorInstance>()
                                                  .PluggedType.ShouldEqual(typeof (Open<string>));
        }

        [Test]
        public void find_family_for_concrete_type_with_default()
        {
            PluginGraph graph = PluginGraph.Empty();
            graph.Families[typeof (BigThingy)].GetDefaultInstance()
                                              .ShouldBeOfType<ConstructorInstance>()
                                              .PluggedType.ShouldEqual(typeof (BigThingy));
        }

        [Test]
        public void find_instance_negative_when_family_does_exist_but_instance_does_not()
        {
            var graph = new PluginGraph();
            graph.Families[typeof (BigThingy)].AddInstance(new SmartInstance<BigThingy>().Named("red"));

            graph.FindInstance(typeof (BigThingy), "blue").ShouldBeNull();
        }

        [Test]
        public void find_instance_negative_when_family_does_not_exist_does_not_create_family()
        {
            var graph = new PluginGraph();
            graph.FindInstance(typeof (BigThingy), "blue").ShouldBeNull();
            graph.Families.Has(typeof (BigThingy)).ShouldBeFalse();
        }

        [Test]
        public void find_instance_positive()
        {
            var graph = new PluginGraph();
            SmartInstance<BigThingy> instance = new SmartInstance<BigThingy>().Named("red");
            graph.Families[typeof (BigThingy)].AddInstance(instance);

            graph.FindInstance(typeof (BigThingy), "red").ShouldBeTheSameAs(instance);
        }

        [Test]
        public void find_instance_can_use_missing_instance()
        {
            var graph = new PluginGraph();
            var instance = new SmartInstance<BigThingy>().Named("red");
            graph.Families[typeof (BigThingy)].MissingInstance = instance;

            graph.FindInstance(typeof (BigThingy), "green")
                 .ShouldBeTheSameAs(instance);
        }


        [Test]
        public void has_default_positive()
        {
            var graph = new PluginGraph();
            graph.Families[typeof(IThingy)].SetDefault(new SmartInstance<BigThingy>());

            graph.HasDefaultForPluginType(typeof (IThingy));
        }

        [Test]
        public void has_default_when_the_family_has_not_been_created()
        {
            var graph = new PluginGraph();
            graph.HasDefaultForPluginType(typeof(IThingy)).ShouldBeFalse();
        }

        [Test]
        public void has_default_with_family_but_no_default()
        {
            var graph = new PluginGraph();
            graph.Families[typeof(IThingy)].AddInstance(new SmartInstance<BigThingy>());
            graph.Families[typeof(IThingy)].AddInstance(new SmartInstance<BigThingy>());

            graph.HasDefaultForPluginType(typeof(IThingy))
                .ShouldBeFalse();
        }

        [Test]
        public void has_instance_negative_when_the_family_has_not_been_created()
        {
            var graph = new PluginGraph();

            graph.HasInstance(typeof(IThingy), "red")
                .ShouldBeFalse();
        }

        [Test]
        public void has_instance_negative_with_the_family_already_existing()
        {
            var graph = new PluginGraph();
            graph.Families[typeof(IThingy)]
                .AddInstance(new SmartInstance<BigThingy>().Named("blue"));

            graph.HasInstance(typeof(IThingy), "red")
                .ShouldBeFalse();
        }

        [Test]
        public void has_instance_positive()
        {
            var graph = new PluginGraph();
            graph.Families[typeof(IThingy)]
                .AddInstance(new SmartInstance<BigThingy>().Named("blue"));

            graph.HasInstance(typeof(IThingy), "blue")
                .ShouldBeTrue();
        }
    }

    public class FakeInstance : Instance, IDisposable
    {
        public bool WasDisposed;

        protected override string getDescription()
        {
            return "fake";
        }

        public void Dispose()
        {
            WasDisposed = true;
        }
    }

    public interface IOpen<T>
    {
    }

    public class Open<T> : IOpen<T>
    {
    }

    //[PluginFamily]
    public interface IThingy
    {
        void Go();
    }

    //[Pluggable("Big")]
    public class BigThingy : IThingy
    {
        #region IThingy Members

        public void Go()
        {
        }

        #endregion
    }
}