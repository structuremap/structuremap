using System;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class BuildStrategiesTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        public class StubInstance : Instance
        {
            private readonly object _constructedObject;


            public StubInstance(object constructedObject)
            {
                _constructedObject = constructedObject;
            }

            protected override object build(Type pluginType, BuildSession session)
            {
                return _constructedObject;
            }

            protected override string getDescription()
            {
                return "Stubbed";
            }
        }

        [Test]
        public void BuildPolicy_should_apply_interception()
        {
            var mocks = new MockRepository();
            var buildSession = mocks.StrictMock<BuildSession>();

            object firstValue = "first";
            object secondValue = "second";
            var instance = new StubInstance(firstValue);
            Type thePluginType = typeof (IGateway);

            using (mocks.Record())
            {
                Expect.Call(buildSession.ApplyInterception(thePluginType, firstValue)).Return(secondValue);
            }

            using (mocks.Playback())
            {
                var policy = new BuildPolicy();
                object returnedObject = policy.Build(buildSession, thePluginType, instance);

                Assert.AreSame(secondValue, returnedObject);
            }
        }

        [Test]
        public void BuildPolicy_should_throw_270_if_interception_fails()
        {
            try
            {
                LiteralInstance instance = new LiteralInstance("something")
                    .OnCreation<object>(delegate { throw new NotImplementedException(); });

                var policy = new BuildPolicy();
                policy.Build(new StubBuildSession(), typeof (string), instance);
            }
            catch (StructureMapException e)
            {
                Assert.AreEqual(270, e.ErrorCode);
            }
        }


        [Test]
        public void CloneHybrid()
        {
            var policy = new HybridBuildPolicy(){InnerPolicy = new BuildPolicy()};

            var clone = (HybridBuildPolicy) policy.Clone();
            Assert.AreNotSame(policy, clone);
            Assert.IsInstanceOfType(typeof (BuildPolicy), clone.InnerPolicy);
        }

        [Test]
        public void CloneSingleton()
        {
            var policy = new SingletonPolicy();

            var clone = (SingletonPolicy) policy.Clone();
            Assert.AreNotSame(policy, clone);
            Assert.IsInstanceOfType(typeof (BuildPolicy), clone.InnerPolicy);
        }


        [Test]
        public void Singleton_build_policy()
        {
            var policy = new SingletonPolicy();
            ConstructorInstance<ColorService> instance1 =
                new ConstructorInstance<ColorService>(() => new ColorService("Red")).WithName("Red");
            ConstructorInstance<ColorService> instance2 =
                new ConstructorInstance<ColorService>(() => new ColorService("Green")).WithName("Green");

            var red1 = (ColorService) policy.Build(new StubBuildSession(), null, instance1);
            var green1 = (ColorService) policy.Build(new StubBuildSession(), null, instance2);
            var red2 = (ColorService) policy.Build(new StubBuildSession(), null, instance1);
            var green2 = (ColorService) policy.Build(new StubBuildSession(), null, instance2);
            var red3 = (ColorService) policy.Build(new StubBuildSession(), null, instance1);
            var green3 = (ColorService) policy.Build(new StubBuildSession(), null, instance2);

            Assert.AreSame(red1, red2);
            Assert.AreSame(red1, red3);
            Assert.AreSame(green1, green2);
            Assert.AreSame(green1, green3);
        }
    }

    [TestFixture]
    public class when_the_singleton_policy_ejects_all
    {
        private SingletonPolicy policy;
        private StubBuildPolicy inner;
        private StubDisposable disposable1;
        private StubDisposable disposable2;


        [SetUp]
        public void SetUp()
        {
            policy = new SingletonPolicy();
            inner = new StubBuildPolicy();
            policy.InnerPolicy = inner;

            disposable1 = new StubDisposable();
            disposable2 = new StubDisposable();

            policy.Cache[new InstanceKey() { Name = "a", PluginType = typeof(IGateway) }] = disposable1;
            policy.Cache[new InstanceKey() {Name = "b", PluginType = typeof (IGateway)}] = disposable2;
            policy.Cache[new InstanceKey() {Name = "c", PluginType = typeof (IGateway)}] = new object();

            policy.EjectAll();
        }

        [Test]
        public void the_count_should_be_zero()
        {
            policy.Cache.Count.ShouldEqual(0);
        }

        [Test]
        public void should_have_called_dispose_if_it_was_there()
        {
            disposable1.DisposedWasCalled.ShouldBeTrue();
            disposable2.DisposedWasCalled.ShouldBeTrue();
        }

        [Test]
        public void inner_policy_was_ejected()
        {
            inner.WasEjected.ShouldBeTrue();
        }
        
    }

    public class StubDisposable : IDisposable
    {
        public bool DisposedWasCalled;

        public void Dispose()
        {
            DisposedWasCalled = true;
        }
    }

    public class StubBuildPolicy : IBuildPolicy
    {
        public bool WasEjected;

        public object Build(BuildSession buildSession, Type pluginType, Instance instance)
        {
            throw new System.NotImplementedException();
        }

        public IBuildPolicy Clone()
        {
            throw new System.NotImplementedException();
        }

        public void EjectAll()
        {
            WasEjected = true;
        }
    }
}