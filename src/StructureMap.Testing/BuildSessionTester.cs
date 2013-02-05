using System;
using System.Linq;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing
{
    [TestFixture]
    public class BuildSessionTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        private void assertActionThrowsErrorCode(int errorCode, Action action)
        {
            try
            {
                action();

                Assert.Fail("Should have thrown StructureMapException");
            }
            catch (StructureMapException ex)
            {
                Assert.AreEqual(errorCode, ex.ErrorCode);
            }
        }

        public class WidgetHolder
        {
            private readonly IWidget[] _widgets;

            public WidgetHolder(IWidget[] widgets)
            {
                _widgets = widgets;
            }

            public IWidget[] Widgets { get { return _widgets; } }
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

                x.ForConcreteType<TopClass>().Configure.OnCreation(
                    (c, top) => { top.Widgets = c.All<IWidget>().ToArray(); });
            });

            container.GetInstance<TopClass>().Widgets.Count().ShouldEqual(4);
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

                x.ForConcreteType<TopClass>().Configure.OnCreation(
                    (c, top) => { top.Widgets = c.All<IWidget>().ToArray(); });
            });

            container.GetInstance<TopClass>().Widgets.Count().ShouldEqual(4);
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

                x.ForConcreteType<TopClass>().Configure.OnCreation(
                    (c, top) => { top.Widgets = c.GetAllInstances<IWidget>().ToArray(); });
            });

            container.GetInstance<TopClass>().Widgets.Count().ShouldEqual(4);
        }

        [Test]
        public void Get_a_unique_value_for_each_individual_buildsession()
        {
            int count = 0;

            var session = BuildSession.Empty();
            var session2 = BuildSession.Empty();
            var instance = new LambdaInstance<ColorRule>(() =>
            {
                count++;
                return new ColorRule("Red");
            });

            object result1 = session.CreateInstance(typeof (ColorRule), instance);
            object result2 = session.CreateInstance(typeof (ColorRule), instance);
            object result3 = session2.CreateInstance(typeof (ColorRule), instance);
            object result4 = session2.CreateInstance(typeof (ColorRule), instance);

            Assert.AreEqual(2, count);

            Assert.AreSame(result1, result2);
            Assert.AreNotSame(result1, result3);
            Assert.AreSame(result3, result4);
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
            Assert.AreEqual(3, holder.Widgets.Length);
        }

        [Test]
        public void Return_the_same_object_everytime_an_object_is_requested()
        {
            int count = 0;

            var session = BuildSession.Empty();
            var instance = new LambdaInstance<ColorRule>(() =>
            {
                count++;
                return new ColorRule("Red");
            });

            object result1 = session.CreateInstance(typeof (ColorRule), instance);
            object result2 = session.CreateInstance(typeof (ColorRule), instance);
            object result3 = session.CreateInstance(typeof (ColorRule), instance);
            object result4 = session.CreateInstance(typeof (ColorRule), instance);

            Assert.AreEqual(1, count);

            Assert.AreSame(result1, result2);
            Assert.AreSame(result1, result3);
            Assert.AreSame(result1, result4);
        }

        [Test]
        public void Return_the_same_object_within_a_session_for_the_default_of_a_plugin_type()
        {
            int count = 0;

            var instance = new LambdaInstance<ColorRule>(() =>
            {
                count++;
                return new ColorRule("Red");
            });
            var registry = new Registry();
            registry.For<ColorRule>().Use(instance);

            PluginGraph graph = registry.Build();
            var session = BuildSession.ForPluginGraph(graph);


            object result1 = session.CreateInstance(typeof (ColorRule));
            object result2 = session.CreateInstance(typeof (ColorRule));
            object result3 = session.CreateInstance(typeof (ColorRule));
            object result4 = session.CreateInstance(typeof (ColorRule));

            Assert.AreEqual(1, count);

            Assert.AreSame(result1, result2);
            Assert.AreSame(result1, result3);
            Assert.AreSame(result1, result4);
        }

        [Test]
        public void Throw_200_When_trying_to_build_an_instance_that_cannot_be_found()
        {
            var graph = new PipelineGraph(new PluginGraph());

            assertActionThrowsErrorCode(200, delegate
            {
                var session = new BuildSession(graph, null);
                session.CreateInstance(typeof (IGateway), "Gateway that is not configured");
            });
        }


        [Test]
        public void when_building_an_instance_use_the_register_the_stack_frame()
        {
            var recordingInstance = new BuildSessionInstance1();
            ConfiguredInstance instance =
                new ConfiguredInstance(typeof (ClassWithRule)).Ctor<Rule>("rule").Is(recordingInstance);
            var session = BuildSession.Empty();

            session.CreateInstance(typeof (IClassWithRule), instance);

            recordingInstance.Root.ConcreteType.ShouldEqual(typeof (ClassWithRule));
            recordingInstance.Root.RequestedType.ShouldEqual(typeof (IClassWithRule));
            recordingInstance.Root.Name.ShouldEqual(instance.Name);

            recordingInstance.Current.ConcreteType.ShouldEqual(typeof (ColorRule));
            recordingInstance.Current.RequestedType.ShouldEqual(typeof (Rule));
            recordingInstance.Current.Name.ShouldEqual(recordingInstance.Name);
        }

        [Test]
        public void When_calling_CreateInstance_if_no_default_can_be_found_throw_202()
        {
            var graph = new PipelineGraph(new PluginGraph());

            assertActionThrowsErrorCode(202, delegate
            {
                var session = new BuildSession(graph, null);
                session.CreateInstance(typeof (IGateway));
            });
        }

        [Test]
        public void when_retrieving_an_object_by_name()
        {
            var red = new ColorService("red");
            var green = new ColorService("green");

            var graph = new PluginGraph();
            PluginFamily family = graph.FindFamily(typeof (IService));
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
			session.GetInstance(typeof(IService), "red").ShouldBeTheSameAs(red);
		}

        [Test]
        public void when_retrieving_by_try_get_instance_for_instance_that_does_exist()
        {
            var session = BuildSession.Empty();
            var theService = new ColorService("red");
            session.RegisterDefault(typeof (IService), theService);

            session.TryGetInstance<IService>().ShouldBeTheSameAs(theService);
        }

        [Test]
        public void when_retrieving_by_try_get_named_instance_that_does_exist()
        {
            var red = new ColorService("red");
            var green = new ColorService("green");

            var graph = new PluginGraph();
            PluginFamily family = graph.FindFamily(typeof (IService));
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

			session.TryGetInstance(typeof(IService)).ShouldBeTheSameAs(theService);
		}

		[Test]
		public void when_retrieving_with_try_get_instance_with_nongeneric_type_that_does_not_exist()
		{
			var session = BuildSession.Empty();
			session.TryGetInstance(typeof(IService)).ShouldBeNull();
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
			session.TryGetInstance(typeof(IService), "red").ShouldBeTheSameAs(red);
		}

    	[Test]
    	public void when_retrieving_by_try_get_named_instance_with_type_that_does_not_exist()
    	{
			var session = BuildSession.Empty();
			session.TryGetInstance(typeof(IService), "yo").ShouldBeNull();
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

        public interface IFooService { }
        public class Service : IFooService { }

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
        public BuildFrame Current { get; set; }
        public BuildFrame Root { get; set; }

        protected override string getDescription()
        {
            return string.Empty;
        }

        protected override Type getConcreteType(Type pluginType)
        {
            return typeof (ColorRule);
        }

        protected override object build(Type pluginType, BuildSession session)
        {
            Current = session.BuildStack.Current;
            Root = session.BuildStack.Root;

            return new ColorRule("Red");
        }
    }
}