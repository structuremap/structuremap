using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class LifecycleObjectCacheTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            cache = new LifecycleObjectCache();
        }

        #endregion

        private LifecycleObjectCache cache;

        [Test]
        public void eject_a_disposable_object()
        {
            var disposable = MockRepository.GenerateMock<IDisposable>();
            var instance = new ObjectInstance(disposable);

            cache.Set(typeof (IWidget), instance, disposable);

            cache.Eject(typeof (IWidget), instance);

            cache.Has(typeof (IWidget), instance).ShouldBeFalse();

            disposable.AssertWasCalled(x => x.Dispose());
        }

        [Test]
        public void eject_a_non_disposable_object()
        {
            var widget = new AWidget();
            var instance = new ObjectInstance(widget);

            cache.Set(typeof (IWidget), instance, widget);

            cache.Eject(typeof (IWidget), instance);

            cache.Has(typeof (IWidget), instance).ShouldBeFalse();
        }

        [Test]
        public void has()
        {
            var widget = new AWidget();
            var instance = new ObjectInstance(widget);

            cache.Has(typeof (IWidget), instance).ShouldBeFalse();

            cache.Set(typeof (Rule), instance, widget);

            cache.Has(typeof (IWidget), instance).ShouldBeFalse();

            cache.Set(typeof (IWidget), new ObjectInstance(new AWidget()), widget);

            cache.Has(typeof (IWidget), instance).ShouldBeFalse();

            cache.Set(typeof (IWidget), instance, widget);

            cache.Has(typeof (IWidget), instance).ShouldBeTrue();
        }

        [Test, Ignore("Not now")]
        public void can_serialize()
        {
            var widget = new ColorWidget("blue");
            var instance = new ObjectInstance(widget);
            cache.Set(typeof(Rule), instance, widget);

            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize(stream, cache);

            stream.Position = 0;

            var deserializedCache = (LifecycleObjectCache)formatter.Deserialize(stream);
            Assert.AreNotSame(cache, deserializedCache);

            var cachedWidget = deserializedCache.Get(typeof(Rule), instance, null) as ColorWidget;
            cachedWidget.ShouldNotBeNull();
            cachedWidget.Color.ShouldEqual("blue");
        }
    }
}