using System;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Graph;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class Add_PluginGraphConfigurationTester
    {
        private class TestGraphConfig : IPluginGraphConfiguration
        {
            public static TestGraphConfig Me;

            public TestGraphConfig()
            {
                Me = this;
            }

            public bool ConfigureWasCalled { get; set; }

            public bool RegisterWasCalled { get; set; }

            void IPluginGraphConfiguration.Configure(PluginGraph graph)
            {
                ConfigureWasCalled = true;
            }

            void IPluginGraphConfiguration.Register(PluginGraphBuilder builder)
            {
                RegisterWasCalled = true;
            }
        }

        [SetUp]
        public void TearDown()
        {
            TestGraphConfig.Me = null;
        }

        [Test]
        public void register_gets_called()
        {
            new Container(ce => ce.RegisterPluginGraphConfiguration<TestGraphConfig>());
            TestGraphConfig.Me.ShouldNotBeNull();
            TestGraphConfig.Me.RegisterWasCalled.ShouldBeTrue();
            TestGraphConfig.Me.ConfigureWasCalled.ShouldBeFalse();
        }

        [Test]
        public void configure_gets_called()
        {
            new Container(ce => ce.ConfigurePluginGraph<TestGraphConfig>());
            TestGraphConfig.Me.ShouldNotBeNull();
            TestGraphConfig.Me.ConfigureWasCalled.ShouldBeTrue();
            TestGraphConfig.Me.RegisterWasCalled.ShouldBeFalse();
        }

    }

}