using System;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.Mementos;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Source;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration
{
    [TestFixture]
    public class NormalGraphBuilderTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void Configure_a_family_that_does_not_exist_and_log_an_error_with_PluginGraph()
        {
            NormalGraphBuilder builder = new NormalGraphBuilder(new Registry[0]);
            builder.ConfigureFamily(new TypePath("a,a"), delegate (PluginFamily f){});

            builder.PluginGraph.Log.AssertHasError(103);
        }

        [Test]
        public void Do_not_call_the_action_on_ConfigureFamily_if_the_type_path_blows_up()
        {
            NormalGraphBuilder builder = new NormalGraphBuilder(new Registry[0]);
            builder.ConfigureFamily(new TypePath("a,a"), delegate(PluginFamily f)
                                                             {
                                                                 Assert.Fail("Should not be called");
                                                             });

        }

        [Test]
        public void Call_the_action_on_configure_family_if_the_pluginType_is_found()
        {
            TypePath typePath = new TypePath(typeof(IGateway));

            bool iWasCalled = false;
            NormalGraphBuilder builder = new NormalGraphBuilder(new Registry[0]);
            builder.ConfigureFamily(typePath, delegate(PluginFamily f)
                                                  {
                                                      Assert.AreEqual(typeof(IGateway), f.PluginType);
                                                      iWasCalled = true;
                                                  });


            Assert.IsTrue(iWasCalled);
        }

        [Test]
        public void Log_an_error_for_a_requested_system_object_if_it_cannot_be_created()
        {
            MemoryInstanceMemento memento = new MemoryInstanceMemento();
            NormalGraphBuilder builder = new NormalGraphBuilder(new Registry[0]);

            builder.WithSystemObject<MementoSource>(memento, "I am going to break here", delegate(MementoSource source){});

            builder.PluginGraph.Log.AssertHasError(130);
        }

        [Test]
        public void Do_not_try_to_execute_the_action_when_requested_system_object_if_it_cannot_be_created()
        {
            MemoryInstanceMemento memento = new MemoryInstanceMemento();
            NormalGraphBuilder builder = new NormalGraphBuilder(new Registry[0]);

            builder.WithSystemObject<MementoSource>(memento, "I am going to break here", delegate(MementoSource source)
                                                                                             {
                                                                                                 Assert.Fail("Wasn't supposed to be called");
                                                                                             });

        }

        [Test]
        public void Create_system_object_successfully_and_call_the_requested_action()
        {
            
            MemoryInstanceMemento memento = new MemoryInstanceMemento("Singleton", "anything");

            bool iWasCalled = false;

            NormalGraphBuilder builder = new NormalGraphBuilder(new Registry[0]);
            builder.PrepareSystemObjects();
            builder.WithSystemObject<IInstanceInterceptor>(memento, "singleton", delegate(IInstanceInterceptor policy)
                                                                             {
                                                                                 Assert.IsInstanceOfType(typeof(SingletonPolicy), policy);
                                                                                 iWasCalled = true;
                                                                             });

            Assert.IsTrue(iWasCalled);
        }

        [Test]
        public void WithType_fails_and_logs_error_with_the_context()
        {
            NormalGraphBuilder builder = new NormalGraphBuilder(new Registry[0]);
            builder.WithType(new TypePath("a,a"), "creating a Plugin", delegate(Type t){Assert.Fail("Should not be called");});

            builder.PluginGraph.Log.AssertHasError(131);
        }

        [Test]
        public void WithType_calls_through_to_the_Action_if_the_type_can_be_found()
        {
            NormalGraphBuilder builder = new NormalGraphBuilder(new Registry[0]);
            bool iWasCalled = true;

            builder.WithType(new TypePath(this.GetType()), "creating a Plugin", delegate(Type t)
                                                                                    {
                                                                                        iWasCalled = true;
                                                                                        Assert.AreEqual(this.GetType(), t);
                                                                                    });

            Assert.IsTrue(iWasCalled);
        }

        [Test]
        public void AddAssembly_HappyPath()
        {
            NormalGraphBuilder builder = new NormalGraphBuilder(new Registry[0]);
            string assemblyName = this.GetType().Assembly.GetName().Name;
            builder.AddAssembly(assemblyName);

            Assert.IsTrue(builder.PluginGraph.Assemblies.Contains(assemblyName));
            Assert.AreEqual(0, builder.PluginGraph.Log.ErrorCount);
        }

        [Test]
        public void AddAssembly_SadPath()
        {
            NormalGraphBuilder builder = new NormalGraphBuilder(new Registry[0]);
            builder.AddAssembly("something");

            builder.PluginGraph.Log.AssertHasError(101);
        }
    }
}
