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

        [Test]
        public void Throw_200_When_trying_to_build_an_instance_that_cannot_be_found()
        {
            PipelineGraph graph = new PipelineGraph(new PluginGraph());

            assertActionThrowsErrorCode(200, delegate()
            {
                BuildSession session = new BuildSession(graph, null);
                session.CreateInstance(typeof (IGateway), "A Name");
            });
        }

        [Test]
        public void When_calling_CreateInstance_if_no_default_can_be_found_throw_202()
        {
            PipelineGraph graph = new PipelineGraph(new PluginGraph());

            assertActionThrowsErrorCode(202, delegate()
            {
                BuildSession session = new BuildSession(graph, null);
                session.CreateInstance(typeof (IGateway));
            });
        }

        [Test]
        public void Return_the_same_object_everytime_an_object_is_requested()
        {
            int count = 0;

            BuildSession session = new BuildSession(new PluginGraph());
            ConstructorInstance instance = new ConstructorInstance(delegate()
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

            ConstructorInstance instance = new ConstructorInstance(delegate()
            {
                count++;
                return new ColorRule("Red");
            });
            Registry registry = new Registry();
            registry.ForRequestedType<ColorRule>().TheDefaultIs(instance);

            PluginGraph graph = registry.Build();
            BuildSession session = new BuildSession(graph);


            object result1 = session.CreateInstance(typeof(ColorRule));
            object result2 = session.CreateInstance(typeof(ColorRule));
            object result3 = session.CreateInstance(typeof(ColorRule));
            object result4 = session.CreateInstance(typeof(ColorRule));

            Assert.AreEqual(1, count);

            Assert.AreSame(result1, result2);
            Assert.AreSame(result1, result3);
            Assert.AreSame(result1, result4);
        }

        [Test]
        public void Get_a_unique_value_for_each_individual_buildsession()
        {
            int count = 0;

            BuildSession session = new BuildSession(new PluginGraph());
            BuildSession session2 = new BuildSession(new PluginGraph());
            ConstructorInstance instance = new ConstructorInstance(delegate()
            {
                count++;
                return new ColorRule("Red");
            });

            object result1 = session.CreateInstance(typeof(ColorRule), instance);
            object result2 = session.CreateInstance(typeof(ColorRule), instance);
            object result3 = session2.CreateInstance(typeof(ColorRule), instance);
            object result4 = session2.CreateInstance(typeof(ColorRule), instance);

            Assert.AreEqual(2, count);

            Assert.AreSame(result1, result2);
            Assert.AreNotSame(result1, result3);
            Assert.AreSame(result3, result4);
        }

        [Test]
        public void If_no_child_array_is_explicitly_defined_return_all_instances()
        {
            IInstanceManager manager = new InstanceManager(delegate(Registry registry)
            {
                registry.AddInstanceOf<IWidget>(new ColorWidget("Red"));
                registry.AddInstanceOf<IWidget>(new ColorWidget("Blue"));
                registry.AddInstanceOf<IWidget>(new ColorWidget("Green"));
            });

            WidgetHolder holder = manager.CreateInstance<WidgetHolder>();
            Assert.AreEqual(3, holder.Widgets.Length);
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
    }
}