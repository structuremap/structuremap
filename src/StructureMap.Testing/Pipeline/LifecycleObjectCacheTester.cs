using Shouldly;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using System;
using System.Threading;
using NSubstitute;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class LifecycleObjectCacheTester
    {
        public LifecycleObjectCacheTester()
        {
            cache = new LifecycleObjectCache();
        }

        private LifecycleObjectCache cache;

        [Fact]
        public void get_for_uncached_instance_returns_passed_instance()
        {
            var aWidget = new AWidget();
            var instance = new ObjectInstance(aWidget);

            var cachedWidget = cache.Get(typeof(IWidget), instance, new StubBuildSession());

            cachedWidget.ShouldBe(aWidget);
        }

        [Fact]
        public void get_for_uncached_instance_builds_instance()
        {
            var instance = new ObjectInstance(new AWidget());
            var mockBuildSession = Substitute.For<IBuildSession>();

            cache.Get(typeof(IWidget), instance, mockBuildSession);

            mockBuildSession.Received().BuildNewInOriginalContext(typeof(IWidget), instance);
        }

        [Fact]
        public void get_for_cached_instance_does_not_build_instance()
        {
            var instance = new ObjectInstance(new AWidget());
            var mockBuildSession = Substitute.For<IBuildSession>();
            cache.Get(typeof(IWidget), instance, new StubBuildSession());

            cache.Get(typeof(IWidget), instance, mockBuildSession);

            mockBuildSession.DidNotReceive().BuildNewInOriginalContext(typeof(IWidget), instance);
        }

        [Fact]
        public void get_for_cached_instance_returns_cached_instance()
        {
            var aWidget = new AWidget();
            var instance = new ObjectInstance(aWidget);
            cache.Get(typeof(IWidget), instance, new StubBuildSession());

            var cachedWidget = cache.Get(typeof(IWidget), instance, new StubBuildSession());

            cachedWidget.ShouldBe(aWidget);
        }

        [Fact]
        public void get_for_cached_instance_created_on_different_thread_returns_cached_instance()
        {
            var aWidget = new AWidget();
            var instance = new ObjectInstance(aWidget);
            cache.Get(typeof(IWidget), instance, new StubBuildSession());

            object cachedWidget = null;
            var thread =
                new Thread(() => { cachedWidget = cache.Get(typeof(IWidget), instance, new StubBuildSession()); });

            thread.Start();
            // Allow 10ms for the thread to start and for Get call to complete
            thread.Join(10);

            cachedWidget.ShouldNotBeNull();
            cachedWidget.ShouldBe(aWidget);
        }

        [Fact]
        public void eject_a_disposable_object()
        {
            var disposable = Substitute.For<IDisposable>();
            var instance = new ObjectInstance(disposable);

            cache.Set(typeof(IWidget), instance, disposable);

            cache.Eject(typeof(IWidget), instance);

            cache.Has(typeof(IWidget), instance).ShouldBeFalse();

            disposable.Received().Dispose();
        }

        [Fact]
        public void eject_a_non_disposable_object()
        {
            var widget = new AWidget();
            var instance = new ObjectInstance(widget);

            cache.Set(typeof(IWidget), instance, widget);

            cache.Eject(typeof(IWidget), instance);

            cache.Has(typeof(IWidget), instance).ShouldBeFalse();
        }

        [Fact]
        public void has()
        {
            var widget = new AWidget();
            var instance = new ObjectInstance(widget);

            cache.Has(typeof(IWidget), instance).ShouldBeFalse();

            cache.Set(typeof(Rule), instance, widget);

            cache.Has(typeof(IWidget), instance).ShouldBeFalse();

            cache.Set(typeof(IWidget), new ObjectInstance(new AWidget()), widget);

            cache.Has(typeof(IWidget), instance).ShouldBeFalse();

            cache.Set(typeof(IWidget), instance, widget);

            cache.Has(typeof(IWidget), instance).ShouldBeTrue();
        }
    }
}