using System;
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

            public IWidget[] Widgets
            {
                get { return _widgets; }
            }
        }

        [Test]
        public void Get_a_unique_value_for_each_individual_buildsession()
        {
            int count = 0;

            BuildSession session = new BuildSession(new PluginGraph());
            BuildSession session2 = new BuildSession(new PluginGraph());
            ConstructorInstance<ColorRule> instance = new ConstructorInstance<ColorRule>(() =>
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
                r.ForRequestedType<IWidget>().AddInstances(x =>
                {
                    x.Object(new ColorWidget("Red"));
                    x.Object(new ColorWidget("Blue"));
                    x.Object(new ColorWidget("Green"));
                });
            });

            WidgetHolder holder = manager.GetInstance<WidgetHolder>();
            Assert.AreEqual(3, holder.Widgets.Length);
        }

        [Test]
        public void Return_the_same_object_everytime_an_object_is_requested()
        {
            int count = 0;

            BuildSession session = new BuildSession(new PluginGraph());
            ConstructorInstance<ColorRule> instance = new ConstructorInstance<ColorRule>(() =>
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

            ConstructorInstance<ColorRule> instance = new ConstructorInstance<ColorRule>( () =>
            {
                count++;
                return new ColorRule("Red");
            });
            Registry registry = new Registry();
            registry.ForRequestedType<ColorRule>().TheDefault.IsThis(instance);

            PluginGraph graph = registry.Build();
            BuildSession session = new BuildSession(graph);


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
            PipelineGraph graph = new PipelineGraph(new PluginGraph());

            assertActionThrowsErrorCode(200, delegate
            {
                BuildSession session = new BuildSession(graph, null);
                session.CreateInstance(typeof (IGateway), "Gateway that is not configured");
            });
        }

        [Test]
        public void When_calling_CreateInstance_if_no_default_can_be_found_throw_202()
        {
            PipelineGraph graph = new PipelineGraph(new PluginGraph());

            assertActionThrowsErrorCode(202, delegate
            {
                BuildSession session = new BuildSession(graph, null);
                session.CreateInstance(typeof (IGateway));
            });
        }



        [Test]
        public void when_building_an_instance_use_the_register_the_stack_frame()
        {
            var recordingInstance = new BuildSessionInstance1();
            ConfiguredInstance instance = new ConfiguredInstance(typeof(ClassWithRule)).Child("rule").Is(recordingInstance);
            BuildSession session = new BuildSession(new PluginGraph());

            session.CreateInstance(typeof (IClassWithRule), instance);

            recordingInstance.Root.ConcreteType.ShouldEqual(typeof(ClassWithRule));
            recordingInstance.Root.RequestedType.ShouldEqual(typeof(IClassWithRule));
            recordingInstance.Root.Name.ShouldEqual(instance.Name);

            recordingInstance.Current.ConcreteType.ShouldEqual(typeof(ColorRule));
            recordingInstance.Current.RequestedType.ShouldEqual(typeof(Rule));
            recordingInstance.Current.Name.ShouldEqual(recordingInstance.Name);
        }
    }

    public interface IClassWithRule{}

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
            return typeof(ColorRule);
        }

        protected override object build(Type pluginType, BuildSession session)
        {
            Current = session.BuildStack.Current;
            Root = session.BuildStack.Root;

            return new ColorRule("Red");
        }
    }
}