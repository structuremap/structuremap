using Shouldly;
using StructureMap.Building;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Pipeline;
using System;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace StructureMap.Testing.Graph
{
    public class PluginGraphTester
    {
        public static PluginGraph Empty()
        {
            return new PluginGraphBuilder().Build();
        }

        [Fact]
        public void default_tracking_style()
        {
            Empty().TransientTracking.ShouldBe(TransientTracking.DefaultNotTrackedAtRoot);
        }

        [Fact]
        public void add_type_adds_an_instance_for_type_once_and_only_once()
        {
            var graph = PluginGraph.CreateRoot();

            graph.AddType(typeof(IThingy), typeof(BigThingy));

            var family = graph.Families[typeof(IThingy)];
            family.Instances
                .Single()
                .ShouldBeOfType<ConstructorInstance>()
                .PluggedType.ShouldBe(typeof(BigThingy));

            graph.AddType(typeof(IThingy), typeof(BigThingy));

            family.Instances.Count().ShouldBe(1);
        }

        [Fact]
        public void all_instances_when_family_has_not_been_created()
        {
            var graph = PluginGraph.CreateRoot();
            graph.AllInstances(typeof(BigThingy)).Any().ShouldBeFalse();

            graph.Families.Has(typeof(BigThingy)).ShouldBeFalse();
        }

        [Fact]
        public void all_instances_when_the_family_already_exists()
        {
            var graph = PluginGraph.CreateRoot();

            // just forcing the family to be created
            graph.Families[typeof(BigThingy)].ShouldNotBeNull();

            graph.AllInstances(typeof(BigThingy)).Any().ShouldBeFalse();
        }

        [Fact]
        public void eject_family_removes_the_family_and_disposes_all_of_its_instances()
        {
            var instance1 = new FakeInstance();
            var instance2 = new FakeInstance();
            var instance3 = new FakeInstance();

            var graph = PluginGraph.CreateRoot();
            graph.Families[typeof(IThingy)].AddInstance(instance1);
            graph.Families[typeof(IThingy)].AddInstance(instance2);
            graph.Families[typeof(IThingy)].AddInstance(instance3);

            graph.EjectFamily(typeof(IThingy));

            instance1.WasDisposed.ShouldBeTrue();
            instance2.WasDisposed.ShouldBeTrue();
            instance3.WasDisposed.ShouldBeTrue();

            graph.Families.Has(typeof(IThingy));
        }

        [Fact]
        public void find_family_by_closing_an_open_interface_that_matches()
        {
            var graph = Empty();
            graph.Families[typeof(IOpen<>)].SetDefault(new ConfiguredInstance(typeof(Open<>)));

            graph.Families[typeof(IOpen<string>)].GetDefaultInstance().ShouldBeOfType<ConstructorInstance>()
                .PluggedType.ShouldBe(typeof(Open<string>));
        }

        [Fact]
        public void find_family_for_concrete_type_with_default()
        {
            var graph = Empty();
            graph.Families[typeof(BigThingy)].GetDefaultInstance()
                .ShouldBeOfType<ConstructorInstance>()
                .PluggedType.ShouldBe(typeof(BigThingy));
        }

        [Fact]
        public void find_instance_negative_when_family_does_exist_but_instance_does_not()
        {
            var graph = PluginGraph.CreateRoot();
            graph.Families[typeof(BigThingy)].AddInstance(new SmartInstance<BigThingy>().Named("red"));

            graph.FindInstance(typeof(BigThingy), "blue").ShouldBeNull();
        }

        [Fact]
        public void find_instance_negative_when_family_does_not_exist_does_not_create_family()
        {
            var graph = PluginGraph.CreateRoot();
            graph.FindInstance(typeof(BigThingy), "blue").ShouldBeNull();
            graph.Families.Has(typeof(BigThingy)).ShouldBeFalse();
        }

        [Fact]
        public void find_instance_positive()
        {
            var graph = PluginGraph.CreateRoot();
            var instance = new SmartInstance<BigThingy>().Named("red");
            graph.Families[typeof(BigThingy)].AddInstance(instance);

            graph.FindInstance(typeof(BigThingy), "red").ShouldBeTheSameAs(instance);
        }

        [Fact]
        public void find_instance_can_use_missing_instance()
        {
            var graph = PluginGraph.CreateRoot();
            var instance = new SmartInstance<BigThingy>().Named("red");
            graph.Families[typeof(BigThingy)].MissingInstance = instance;

            var cloned_and_renamed = graph.FindInstance(typeof(BigThingy), "green").ShouldBeAssignableTo<ConfiguredInstance>();

            cloned_and_renamed.Name.ShouldBe("green");
            cloned_and_renamed.PluggedType.ShouldBe(typeof(BigThingy));
        }

        [Fact]
        public void has_default_positive()
        {
            var graph = PluginGraph.CreateRoot();
            graph.Families[typeof(IThingy)].SetDefault(new SmartInstance<BigThingy>());

            graph.HasDefaultForPluginType(typeof(IThingy));
        }

        [Fact]
        public void has_default_when_the_family_has_not_been_created()
        {
            var graph = PluginGraph.CreateRoot();
            graph.HasDefaultForPluginType(typeof(IThingy)).ShouldBeFalse();
        }

        [Fact]
        public void has_default_with_family_but_no_default()
        {
            var graph = PluginGraph.CreateRoot();
            graph.Families[typeof(IThingy)].AddInstance(new SmartInstance<BigThingy>());
            graph.Families[typeof(IThingy)].AddInstance(new SmartInstance<BigThingy>());

            graph.HasDefaultForPluginType(typeof(IThingy))
                .ShouldBeFalse();
        }

        [Fact]
        public void has_instance_negative_when_the_family_has_not_been_created()
        {
            var graph = PluginGraph.CreateRoot();

            graph.HasInstance(typeof(IThingy), "red")
                .ShouldBeFalse();
        }

        [Fact]
        public void has_instance_negative_with_the_family_already_existing()
        {
            var graph = PluginGraph.CreateRoot();
            graph.Families[typeof(IThingy)]
                .AddInstance(new SmartInstance<BigThingy>().Named("blue"));

            graph.HasInstance(typeof(IThingy), "red")
                .ShouldBeFalse();
        }

        [Fact]
        public void has_instance_positive()
        {
            var graph = PluginGraph.CreateRoot();
            graph.Families[typeof(IThingy)]
                .AddInstance(new SmartInstance<BigThingy>().Named("blue"));

            graph.HasInstance(typeof(IThingy), "blue")
                .ShouldBeTrue();
        }

        [Fact]
        public void has_family_false_with_simple()
        {
            var graph = Empty();
            graph.HasFamily(typeof(IThingy)).ShouldBeFalse();
        }

        [Fact]
        public void has_family_true_with_simple()
        {
            var graph = Empty();
            graph.AddFamily(new PluginFamily(typeof(IThingy)));

            graph.HasFamily(typeof(IThingy)).ShouldBeTrue();
        }

        [Fact]
        public void add_family_sets_the_parent_relationship()
        {
            var graph = Empty();
            graph.AddFamily(new PluginFamily(typeof(IThingy)));

            graph.Families[typeof(IThingy)].Owner.ShouldBeTheSameAs(graph);
        }

        [Fact]
        public void find_root()
        {
            var top = PluginGraph.CreateRoot();
            var node = top.Profile("Foo");
            var leaf = node.Profile("Bar");

            top.Root.ShouldBeTheSameAs(top);
            node.Root.ShouldBeTheSameAs(top);
            leaf.Root.ShouldBeTheSameAs(top);
        }

        [Fact]
        public void has_family_true_with_open_generics()
        {
            var graph = Empty();
            graph.Families[typeof(IOpen<>)].SetDefault(new ConstructorInstance(typeof(Open<>)));

            graph.HasFamily(typeof(IOpen<string>))
                .ShouldBeTrue();
        }
    }

    public class FakeDependencySource : IDependencySource
    {
        public FakeDependencySource()
        {
            ReturnedType = typeof(string);
        }

        public string Description { get; private set; }

        public Expression ToExpression(ParameterExpression session, ParameterExpression context)
        {
            throw new NotImplementedException();
        }

        public Type ReturnedType { get; }

        public void AcceptVisitor(IDependencyVisitor visitor)
        {
            visitor.Dependency(this);
        }
    }

    public class FakeInstance : Instance, IDisposable
    {
        public bool WasDisposed;

        public readonly FakeDependencySource DependencySource = new FakeDependencySource();

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            return DependencySource;
        }

        public override Type ReturnedType
        {
            get { return null; }
        }

        public override string Description
        {
            get { return "fake"; }
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

        #endregion IThingy Members
    }
}