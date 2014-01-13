using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.DocumentationExamples;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class Use_if_none_tests
    {
        [TestFixture]
        public class For_use_baseline_behavior
        {
            private class Reg1 : Registry
            {
                public Reg1()
                {
                    For<IService>().Use<WhateverService>();
                }
            }

            private class Reg2 : Registry
            {
                public Reg2()
                {
                    For<IService>().Use<RemoteService>();
                }
            }

            [Test]
            public void default_plugin_depends_on_registration_sequence_take1()
            {
                var c = GetContainerWithRegistries<Reg1, Reg2>();
                c.GetInstance<IService>().ShouldBeOfType<RemoteService>();
            }

            [Test]
            public void default_plugin_depends_on_registration_sequence_take2()
            {
                var c = GetContainerWithRegistries<Reg2, Reg1>();
                c.GetInstance<IService>().ShouldBeOfType<WhateverService>();
            }

            [Test]
            public void what_happens_after_eject_all()
            {
                var c = GetContainerWithRegistries<Reg1, Reg2>();
                c.EjectAllInstancesOf<IService>();

                Exception<StructureMapException>.ShouldBeThrownBy(() => c.GetInstance<IService>())
                    .Title.ShouldContain("No default");
            }

            [Test]
            public void no_default_throws()
            {
                Exception<StructureMapException>.ShouldBeThrownBy(() => {
                    new Container().GetInstance<IService>();
                }).Title.ShouldContain("No default");

            }
        }

        private class Reg1 : Registry
        {
            public Reg1()
            {
                For<IService>().UseIfNone<WhateverService>();
            }
        }

        private class Reg2 : Registry
        {
            public Reg2()
            {
                For<IService>().Use<RemoteService>();
            }
        }

        [Test]
        public void the_fallback_service_is_picked_up()
        {
            var c = new Container(ce => ce.AddRegistry<Reg1>());
            c.GetInstance<IService>().ShouldBeOfType<WhateverService>();
        }

        [Test]
        public void fallback_service_ignored_take1()
        {
            var c = GetContainerWithRegistries<Reg1, Reg2>();
            c.GetInstance<IService>().ShouldBeOfType<RemoteService>();
        }

        [Test]
        public void fallback_service_ignored_take2()
        {
            var c = GetContainerWithRegistries<Reg2, Reg1>();
            c.GetInstance<IService>().ShouldBeOfType<RemoteService>();
        }

        [Test]
        public void eject_also_removed_use_if_none_instance()
        {
            var ex = Exception<StructureMapConfigurationException>.ShouldBeThrownBy(() => {
                var c = new Container(ce => ce.AddRegistry<Reg1>());
                c.EjectAllInstancesOf<IService>();

                c.GetInstance<IService>();
            });

            ex.Title.ShouldEqual("No default Instance is registered and cannot be automatically determined for type 'StructureMap.Testing.Widget3.IService'");
        }

        private static IContainer GetContainerWithRegistries<TReg1, TReg2>() where TReg1 : Registry, new()
            where TReg2 : Registry, new()
        {
            return new Container(ce => {
                ce.AddRegistry<TReg1>();
                ce.AddRegistry<TReg2>();
            });
        }
    }
}