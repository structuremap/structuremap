using Shouldly;
using StructureMap.Pipeline;
using System;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class TrackingTransientCacheTester
    {
        [Fact]
        public void get_builds_a_new_object_everytime_it_is_called()
        {
            var cache = new TrackingTransientCache();

            var instance = new SmartInstance<Target>();
            var session = new FakeBuildSession();

            var t1 = cache.Get(typeof(Target), instance, session);
            var t2 = cache.Get(typeof(Target), instance, session);
            var t3 = cache.Get(typeof(Target), instance, session);

            t1.ShouldNotBeTheSameAs(t2);
            t1.ShouldNotBeTheSameAs(t3);
            t2.ShouldNotBeTheSameAs(t3);
        }

        [Fact]
        public void tracks_disposable_objects_only()
        {
            var cache = new TrackingTransientCache();
            var session = new FakeBuildSession();

            var disposable = cache.Get(typeof(Target), new SmartInstance<Target>(), session);
            var notDisposable = cache.Get(typeof(NotTracked), new SmartInstance<NotTracked>(), session);

            cache.Tracked.Single().ShouldBeTheSameAs(disposable);
        }

        [Fact]
        public void is_explicit_release_mode()
        {
            var cache = new TrackingTransientCache();
            cache.Style.ShouldBe(TransientTracking.ExplicitReleaseMode);
        }

        [Fact]
        public void release_from_the_cache()
        {
            var cache = new TrackingTransientCache();

            var instance = new SmartInstance<Target>();
            var session = new FakeBuildSession();

            var t1 = cache.Get(typeof(Target), instance, session).As<Target>();
            var t2 = cache.Get(typeof(Target), instance, session).As<Target>();
            var t3 = cache.Get(typeof(Target), instance, session).As<Target>();

            // pre-conditions
            t2.WasDisposed.ShouldBeFalse();
            cache.Tracked.ShouldHaveTheSameElementsAs(t1, t2, t3);

            cache.Release(t2);

            // only t2 should be disposed
            t1.WasDisposed.ShouldBeFalse();
            t2.WasDisposed.ShouldBeTrue();
            t3.WasDisposed.ShouldBeFalse();

            // t2 should be removed from tracked
            cache.Tracked.ShouldHaveTheSameElementsAs(t1, t3);
        }

        [Fact]
        public void dispose_and_clear_all()
        {
            var cache = new TrackingTransientCache();

            var instance = new SmartInstance<Target>();
            var session = new FakeBuildSession();

            var t1 = cache.Get(typeof(Target), instance, session).As<Target>();
            var t2 = cache.Get(typeof(Target), instance, session).As<Target>();
            var t3 = cache.Get(typeof(Target), instance, session).As<Target>();

            cache.DisposeAndClear();

            cache.Tracked.Any().ShouldBeFalse();

            t1.WasDisposed.ShouldBeTrue();
            t2.WasDisposed.ShouldBeTrue();
            t3.WasDisposed.ShouldBeTrue();
        }
    }

    public class Target : IDisposable
    {
        public bool WasDisposed;

        public void Dispose()
        {
            WasDisposed = true;
        }
    }

    public class NotTracked { }

    public class FakeBuildSession : IBuildSession
    {
        public object BuildNewInSession(Type pluginType, Instance instance)
        {
            return Activator.CreateInstance(pluginType);
        }

        public object BuildNewInOriginalContext(Type pluginType, Instance instance)
        {
            throw new NotImplementedException();
        }

        public object ResolveFromLifecycle(Type pluginType, Instance instance)
        {
            throw new NotImplementedException();
        }

        public Policies Policies { get; private set; }

        public object CreateInstance(Type pluginType, string name)
        {
            throw new NotImplementedException();
        }

        public void Push(Instance instance)
        {
            throw new NotImplementedException();
        }

        public void Pop()
        {
            throw new NotImplementedException();
        }

        public object BuildUnique(Type pluginType, Instance instance)
        {
            throw new NotImplementedException();
        }
    }
}