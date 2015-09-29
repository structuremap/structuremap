using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using StructureMap.Building;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Configuration.DSL;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing
{
    [TestFixture]
    public class BuildSessionTester
    {
        public class WidgetHolder
        {
            private readonly IWidget[] _widgets;

            public WidgetHolder(IWidget[] widgets)
            {
                _widgets = widgets;
            }

            public IWidget[] Widgets
            {
                get { return _widgets; }
            }
        }

        [Test]
        public void can_get_all_of_a_type_during_object_creation()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().AddInstances(o =>
                {
                    o.Type<AWidget>();
                    o.ConstructedBy(() => new ColorWidget("red"));
                    o.ConstructedBy(() => new ColorWidget("blue"));
                    o.ConstructedBy(() => new ColorWidget("green"));
                });

                x.ForConcreteType<TopClass>().Configure.OnCreation("test",
                    (c, top) => { top.Widgets = c.All<IWidget>().ToArray(); });
            });

            container.GetInstance<TopClass>().Widgets.Count().ShouldBe(4);
        }

        [Test]
        public void descriptive_exception_when_a_named_instance_cannot_be_found()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().AddInstances(o =>
                {
                    o.Type<AWidget>();
                    o.ConstructedBy(() => new ColorWidget("red")).Named("red");
                    o.ConstructedBy(() => new ColorWidget("blue")).Named("blue");
                    o.ConstructedBy(() => new ColorWidget("green")).Named("green");
                });
            });

            var ex =
                Exception<StructureMapConfigurationException>.ShouldBeThrownBy(
                    () => { container.GetInstance<IWidget>("purple"); });

            Debug.WriteLine(ex.ToString());

            ex.Context.ShouldContain("The current configuration for type StructureMap.Testing.Widget.IWidget is:");

            ex.Title.ShouldBe(
                "Could not find an Instance named 'purple' for PluginType StructureMap.Testing.Widget.IWidget");
        }


        [Test]
        public void can_get_all_of_a_type_during_object_creation_as_generic_type()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().AddInstances(o =>
                {
                    o.Type<AWidget>();
                    o.ConstructedBy(() => new ColorWidget("red"));
                    o.ConstructedBy(() => new ColorWidget("blue"));
                    o.ConstructedBy(() => new ColorWidget("green"));
                });

                x.ForConcreteType<TopClass>().Configure.OnCreation("set the widgets",
                    (c, top) => { top.Widgets = c.All<IWidget>().ToArray(); });
            });

            container.GetInstance<TopClass>().Widgets.Count().ShouldBe(4);
        }


        [Test]
        public void can_get_all_of_a_type_by_GetAllInstances_during_object_creation_as_generic_type()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().AddInstances(o =>
                {
                    o.Type<AWidget>();
                    o.ConstructedBy(() => new ColorWidget("red"));
                    o.ConstructedBy(() => new ColorWidget("blue"));
                    o.ConstructedBy(() => new ColorWidget("green"));
                });

                x.ForConcreteType<TopClass>().Configure.OnCreation("test",
                    (c, top) => { top.Widgets = c.GetAllInstances<IWidget>().ToArray(); });
            });

            container.GetInstance<TopClass>().Widgets.Count().ShouldBe(4);
        }

        [Test]
        public void Get_a_unique_value_for_each_individual_buildsession()
        {
            var count = 0;

            var session = BuildSession.Empty();
            var session2 = BuildSession.Empty();
            var instance = new LambdaInstance<ColorRule>("counting", () =>
            {
                count++;
                return new ColorRule("Red");
            });

            var result1 = session.FindObject(typeof (ColorRule), instance);
            var result2 = session.FindObject(typeof (ColorRule), instance);
            var result3 = session2.FindObject(typeof (ColorRule), instance);
            var result4 = session2.FindObject(typeof (ColorRule), instance);

            count.ShouldBe(2);

            result1.ShouldBeTheSameAs(result2);
            result1.ShouldNotBeTheSameAs(result3);
            result3.ShouldBeTheSameAs(result4);
        }

        [Test]
        public void If_no_child_array_is_explicitly_defined_return_all_instances()
        {
            IContainer manager = new Container(r =>
            {
                r.For<IWidget>().AddInstances(x =>
                {
                    x.Object(new ColorWidget("Red"));
                    x.Object(new ColorWidget("Blue"));
                    x.Object(new ColorWidget("Green"));
                });
            });

            var holder = manager.GetInstance<WidgetHolder>();
            holder.Widgets.Length.ShouldBe(3);
        }

        [Test]
        public void Return_the_same_object_everytime_an_object_is_requested()
        {
            var count = 0;

            var session = BuildSession.Empty();
            var instance = new LambdaInstance<ColorRule>("counting", () =>
            {
                count++;
                return new ColorRule("Red");
            });

            var result1 = session.FindObject(typeof (ColorRule), instance);
            var result2 = session.FindObject(typeof (ColorRule), instance);
            var result3 = session.FindObject(typeof (ColorRule), instance);
            var result4 = session.FindObject(typeof (ColorRule), instance);

            count.ShouldBe(1);

            result1.ShouldBeTheSameAs(result2);
            result1.ShouldBeTheSameAs(result3);
            result1.ShouldBeTheSameAs(result4);
        }

        [Test]
        public void Return_the_same_object_within_a_session_for_the_default_of_a_plugin_type()
        {
            var count = 0;

            var instance = new LambdaInstance<ColorRule>("counting", () =>
            {
                count++;
                return new ColorRule("Red");
            });
            var registry = new Registry();
            registry.For<ColorRule>().UseInstance(instance);

            var graph = registry.Build();
            var session = BuildSession.ForPluginGraph(graph);


            var result1 = session.GetInstance(typeof (ColorRule));
            var result2 = session.GetInstance(typeof (ColorRule));
            var result3 = session.GetInstance(typeof (ColorRule));
            var result4 = session.GetInstance(typeof (ColorRule));

            count.ShouldBe(1);

            result1.ShouldBeTheSameAs(result2);
            result1.ShouldBeTheSameAs(result3);
            result1.ShouldBeTheSameAs(result4);
        }

        [Test]
        public void Throw_exception_When_trying_to_build_an_instance_that_cannot_be_found()
        {
            var graph = PipelineGraph.BuildEmpty();

            var ex = Exception<StructureMapConfigurationException>.ShouldBeThrownBy(() =>
            {
                var session = new BuildSession(graph);
                session.CreateInstance(typeof (IGateway), "Gateway that is not configured");
            });

            ex.Title.ShouldBe(
                "Could not find an Instance named 'Gateway that is not configured' for PluginType StructureMap.Testing.Widget3.IGateway");
        }


        [Test]
        public void When_calling_GetInstance_if_no_default_can_be_found_throw_202()
        {
            var graph = PipelineGraph.BuildEmpty();


            var ex = Exception<StructureMapConfigurationException>.ShouldBeThrownBy(() =>
            {
                var session = new BuildSession(graph);
                session.GetInstance(typeof (IGateway));
            });

            ex.Context.ShouldContain("There is no configuration specified for StructureMap.Testing.Widget3.IGateway");

            ex.Title.ShouldBe(
                "No default Instance is registered and cannot be automatically determined for type 'StructureMap.Testing.Widget3.IGateway'");
        }

        [Test]
        public void when_retrieving_an_object_by_name()
        {
            var red = new ColorService("red");
            var green = new ColorService("green");

            var graph = PluginGraph.CreateRoot();
            var family = graph.Families[typeof (IService)];
            family.AddInstance(new ObjectInstance(red).Named("red"));
            family.AddInstance(new ObjectInstance(green).Named("green"));

            var session = BuildSession.ForPluginGraph(graph);
            session.GetInstance<IService>("red").ShouldBeTheSameAs(red);
        }

        [Test]
        public void when_retrieving_an_object_by_nongeneric_type_and_name()
        {
            var red = new ColorService("red");
            var green = new ColorService("green");

            var registry = new Registry();
            registry.For<IService>().Add(red).Named("red");
            registry.For<IService>().Add(green).Named("green");
            var graph = registry.Build();

            var session = BuildSession.ForPluginGraph(graph);
            session.GetInstance(typeof (IService), "red").ShouldBeTheSameAs(red);
        }

        [Test]
        public void when_retrieving_by_try_get_instance_for_instance_that_does_exist()
        {
            var theService = new ColorService("red");
            var session = BuildSession.Empty(new ExplicitArguments().Set<IService>(theService));

            session.TryGetInstance<IService>().ShouldBeTheSameAs(theService);
        }

        [Test]
        public void when_retrieving_by_try_get_named_instance_that_does_exist()
        {
            var red = new ColorService("red");
            var green = new ColorService("green");

            var graph = PluginGraph.CreateRoot();
            var family = graph.Families[typeof (IService)];
            family.AddInstance(new ObjectInstance(red).Named("red"));
            family.AddInstance(new ObjectInstance(green).Named("green"));

            var session = BuildSession.ForPluginGraph(graph);
            session.TryGetInstance<IService>("red").ShouldBeTheSameAs(red);
            session.TryGetInstance<IService>("green").ShouldBeTheSameAs(green);
        }

        [Test]
        public void when_retrieving_by_try_get_named_instance_that_does_not_exist()
        {
            var session = BuildSession.Empty();
            session.TryGetInstance<IService>("red").ShouldBeNull();
        }

        [Test]
        public void when_retrieving_with_try_get_instance_for_instance_that_does_not_exists()
        {
            var session = BuildSession.Empty();
            session.TryGetInstance<IService>().ShouldBeNull();
        }

        [Test]
        public void when_retrieving_with_try_get_instance_with_nongeneric_type_that_does_exist()
        {
            var theService = new ColorService("red");
            var registry = new Registry();
            registry.For<IService>().Use(theService);
            var session = BuildSession.ForPluginGraph(registry.Build());

            session.TryGetInstance(typeof (IService)).ShouldBeTheSameAs(theService);
        }

        [Test]
        public void when_retrieving_with_try_get_instance_with_nongeneric_type_that_does_not_exist()
        {
            var session = BuildSession.Empty();
            session.TryGetInstance(typeof (IService)).ShouldBeNull();
        }

        [Test]
        public void when_retrieving_by_try_get_named_instance_with_nongeneric_type_that_does_exist()
        {
            var red = new ColorService("red");
            var green = new ColorService("green");

            var registry = new Registry();
            registry.For<IService>().Add(red).Named("red");
            registry.For<IService>().Add(green).Named("green");
            var graph = registry.Build();

            var session = BuildSession.ForPluginGraph(graph);
            session.TryGetInstance(typeof (IService), "red").ShouldBeTheSameAs(red);
        }

        [Test]
        public void when_retrieving_by_try_get_named_instance_with_type_that_does_not_exist()
        {
            var session = BuildSession.Empty();
            session.TryGetInstance(typeof (IService), "yo").ShouldBeNull();
        }

        [Test]
        public void Can_get_an_instance_using_the_non_generic_method()
        {
            var registry = new Registry();
            registry.For<IFooService>().Use<Service>();

            var graph = registry.Build();

            var session = BuildSession.ForPluginGraph(graph);

            var instance = session.GetInstance(typeof (IFooService));

            instance.ShouldNotBeNull();
            instance.ShouldBeOfType<Service>();
        }

        [Test]
        public void parent_type_is_null_in_the_initial_state()
        {
            var session = BuildSession.ForPluginGraph(PluginGraph.CreateRoot());
            session.ParentType.ShouldBeNull();
        }

        [Test]
        public void push_an_instance_onto_a_session()
        {
            var session = BuildSession.ForPluginGraph(PluginGraph.CreateRoot());
            session.Push(new LambdaInstance<StubbedGateway>(c => new StubbedGateway()));

            session.ParentType.ShouldBeNull();
            session.Push(new SmartInstance<ARule>());

            session.ParentType.ShouldBe(typeof (StubbedGateway));

            session.Push(new SmartInstance<AWidget>());

            session.ParentType.ShouldBe(typeof (ARule));
        }

        [Test]
        public void push_and_pop_an_instance_onto_a_session()
        {
            var session = BuildSession.ForPluginGraph(PluginGraph.CreateRoot());

            session.Push(new SmartInstance<AWidget>());
            session.Push(new LambdaInstance<StubbedGateway>(c => new StubbedGateway()));

            session.Push(new SmartInstance<ARule>());

            session.Pop();

            session.ParentType.ShouldBe(typeof (AWidget));
        }

        [Test]
        public void pushing_the_same_instance_will_throw_a_bidirectional_dependency_exception()
        {
            var session = BuildSession.ForPluginGraph(PluginGraph.CreateRoot());

            var instance1 = new SmartInstance<StubbedGateway>();
            var instance2 = new SmartInstance<ARule>();
            var instance3 = new SmartInstance<AWidget>();

            session.Push(instance1);
            session.Push(instance2);
            session.Push(instance3);

            var ex = Exception<StructureMapBuildException>.ShouldBeThrownBy(() => { session.Push(instance1); });

            ex.Message.ShouldContain("Bi-directional dependency relationship detected!");
        }

        public interface IFooService
        {
        }

        public class Service : IFooService
        {
        }
    }

    public class TopClass
    {
        public TopClass(ClassWithWidget classWithWidget)
        {
        }

        public IWidget[] Widgets { get; set; }
    }

    public class ClassWithWidget
    {
        public ClassWithWidget(IWidget[] widgets)
        {
        }
    }

    public interface IClassWithRule
    {
    }

    public class ClassWithRule : IClassWithRule
    {
        public ClassWithRule(Rule rule)
        {
        }
    }

    public class BuildSessionInstance1 : Instance
    {
        public override string Description
        {
            get { return string.Empty; }
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            return new Constant(pluginType, new ColorRule("Red"));
        }

        public override Type ReturnedType
        {
            get { return typeof (ColorRule); }
        }
    }
}