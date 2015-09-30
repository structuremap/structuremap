using System;
using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;
using Shouldly;
using StructureMap.Pipeline;
using StructureMap.Testing.Acceptance;

namespace StructureMap.Testing
{
    [TestFixture, Explicit("These fail on CI for some weird reason. I've got nothing.")]
    public class disposed_container_message_tester
    {
        private void shouldThrow(Action action)
        {
            Exception<ObjectDisposedException>.ShouldBeThrownBy(action)
                .ObjectName.ShouldBe(ObjectName);
        }

        public string ObjectName = "WRONG";

        [Test]
        public void call_into_disposed_app_container()
        {
            var container = new Container();
            container.Dispose();
            ObjectName = "StructureMap Application Root Container";

            shouldThrow(() => container.BuildUp(this));
            shouldThrow(() => container.AssertConfigurationIsValid());
            shouldThrow(() => container.Configure(_ => {}));
            shouldThrow(() => container.EjectAllInstancesOf<IService>());
            shouldThrow(() => container.Inject("foo"));
            shouldThrow(() => container.Release(this));
            shouldThrow(() => container.CreateChildContainer());
            shouldThrow(() => container.ForGenericType(typeof(GenericsAcceptanceTester.IService<>)));
            shouldThrow(() => container.ForObject(this));
            shouldThrow(() => container.GetAllInstances<IService>());
            shouldThrow(() => container.GetAllInstances(typeof(IService)));
            shouldThrow(() => container.GetAllInstances<IService>(new ExplicitArguments()));
            shouldThrow(() => container.GetAllInstances(typeof(IService),new ExplicitArguments()));
            shouldThrow(() => container.GetInstance(typeof(IService)));
            shouldThrow(() => container.GetInstance(typeof(IService), "blue"));
            shouldThrow(() => container.GetInstance<IService>("blue"));
            shouldThrow(() => container.GetNestedContainer());
            shouldThrow(() => container.GetProfile("colors"));
            shouldThrow(() => Debug.WriteLine(container.Model));
            shouldThrow(() => container.TryGetInstance(typeof(IService)));
            shouldThrow(() => container.TryGetInstance(typeof(IService), "foo"));
            shouldThrow(() => container.WhatDoIHave());


        }


        [Test]
        public void call_into_disposed_nested_container()
        {
            var root = new Container();
            var container = root.GetNestedContainer();
            container.Dispose();
            ObjectName = "StructureMap Nested Container";

            shouldThrow(() => container.BuildUp(this));
            shouldThrow(() => container.AssertConfigurationIsValid());
            shouldThrow(() => container.Configure(_ => { }));
            shouldThrow(() => container.EjectAllInstancesOf<IService>());
            shouldThrow(() => container.Inject("foo"));
            shouldThrow(() => container.Release(this));
            shouldThrow(() => container.CreateChildContainer());
            shouldThrow(() => container.ForGenericType(typeof(GenericsAcceptanceTester.IService<>)));
            shouldThrow(() => container.ForObject(this));
            shouldThrow(() => container.GetAllInstances<IService>());
            shouldThrow(() => container.GetAllInstances(typeof(IService)));
            shouldThrow(() => container.GetAllInstances<IService>(new ExplicitArguments()));
            shouldThrow(() => container.GetAllInstances(typeof(IService), new ExplicitArguments()));
            shouldThrow(() => container.GetInstance(typeof(IService)));
            shouldThrow(() => container.GetInstance(typeof(IService), "blue"));
            shouldThrow(() => container.GetInstance<IService>("blue"));
            shouldThrow(() => container.GetNestedContainer());
            shouldThrow(() => container.GetProfile("colors"));
            shouldThrow(() => Debug.WriteLine(container.Model));
            shouldThrow(() => container.TryGetInstance(typeof(IService)));
            shouldThrow(() => container.TryGetInstance(typeof(IService), "foo"));
            shouldThrow(() => container.WhatDoIHave());


        }

        [Test]
        public void call_into_disposed_child_container()
        {
            var root = new Container();
            var container = root.CreateChildContainer();
            container.Dispose();
            ObjectName = "StructureMap Child/Profile Container";

            shouldThrow(() => container.BuildUp(this));
            shouldThrow(() => container.AssertConfigurationIsValid());
            shouldThrow(() => container.Configure(_ => { }));
            shouldThrow(() => container.EjectAllInstancesOf<IService>());
            shouldThrow(() => container.Inject("foo"));
            shouldThrow(() => container.Release(this));
            shouldThrow(() => container.CreateChildContainer());
            shouldThrow(() => container.ForGenericType(typeof(GenericsAcceptanceTester.IService<>)));
            shouldThrow(() => container.ForObject(this));
            shouldThrow(() => container.GetAllInstances<IService>());
            shouldThrow(() => container.GetAllInstances(typeof(IService)));
            shouldThrow(() => container.GetAllInstances<IService>(new ExplicitArguments()));
            shouldThrow(() => container.GetAllInstances(typeof(IService), new ExplicitArguments()));
            shouldThrow(() => container.GetInstance(typeof(IService)));
            shouldThrow(() => container.GetInstance(typeof(IService), "blue"));
            shouldThrow(() => container.GetInstance<IService>("blue"));
            shouldThrow(() => container.GetNestedContainer());
            shouldThrow(() => container.GetProfile("colors"));
            shouldThrow(() => Debug.WriteLine(container.Model));
            shouldThrow(() => container.TryGetInstance(typeof(IService)));
            shouldThrow(() => container.TryGetInstance(typeof(IService), "foo"));
            shouldThrow(() => container.WhatDoIHave());


        }
    }
}