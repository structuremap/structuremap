using Shouldly;
using StructureMap.Pipeline;
using System;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Acceptance
{
    public class transient_tracking_mode_specs
    {
        [Fact]
        public void Bug_500_release_transient_created_by_root_container()
        {
            var container = new Container();
            container.Configure(_ => _.TransientTracking = TransientTracking.ExplicitReleaseMode);

            container.TransientTracking.ShouldBeOfType<TrackingTransientCache>();

            var transient1 = container.GetInstance<DisposableGuy>();
            var transient2 = container.GetInstance<DisposableGuy>();

            transient1.WasDisposed.ShouldBeFalse();
            transient2.WasDisposed.ShouldBeFalse();

            // release ONLY transient2
            container.Release(transient2);

            transient1.WasDisposed.ShouldBeFalse();
            transient2.WasDisposed.ShouldBeTrue();

            // transient2 should no longer be
            // tracked by the container
            container.TransientTracking.Tracked
                .Single()
                .ShouldBeTheSameAs(transient1);
        }

        // SAMPLE: transient_tracking_mode
        [Fact]
        public void release_transient_created_by_root_container()
        {
            var container = new Container(_ => _.TransientTracking = TransientTracking.ExplicitReleaseMode);

            container.TransientTracking.ShouldBeOfType<TrackingTransientCache>();

            var transient1 = container.GetInstance<DisposableGuy>();
            var transient2 = container.GetInstance<DisposableGuy>();

            transient1.WasDisposed.ShouldBeFalse();
            transient2.WasDisposed.ShouldBeFalse();

            // release ONLY transient2
            container.Release(transient2);

            transient1.WasDisposed.ShouldBeFalse();
            transient2.WasDisposed.ShouldBeTrue();

            // transient2 should no longer be
            // tracked by the container
            container.TransientTracking.Tracked
                .Single()
                .ShouldBeTheSameAs(transient1);
        }

        [Fact]
        public void disposing_the_container_disposes_tracked_transients()
        {
            var container = new Container(_ => _.TransientTracking = TransientTracking.ExplicitReleaseMode);

            var transient1 = container.GetInstance<DisposableGuy>();
            var transient2 = container.GetInstance<DisposableGuy>();

            transient1.WasDisposed.ShouldBeFalse();
            transient2.WasDisposed.ShouldBeFalse();

            container.Dispose();

            transient1.WasDisposed.ShouldBeTrue();
            transient2.WasDisposed.ShouldBeTrue();
        }

        // ENDSAMPLE

        [Fact]
        public void tracks_transients_built_as_dependencies_in_graph()
        {
            var container = new Container(_ => _.TransientTracking = TransientTracking.ExplicitReleaseMode);

            var root = container.GetInstance<Grandparent>();
            var parent = root.Parent;
            var guy = parent.Guy;

            container.TransientTracking.Tracked.ShouldHaveTheSameElementsAs(guy, parent);

            container.Dispose();

            parent.WasDisposed.ShouldBeTrue();
            guy.WasDisposed.ShouldBeTrue();
        }

        [Fact]
        public void disposing_a_child_container_disposes_the_objects_it_has_created_by_not_the_parent_created_objects()
        {
            var root = new Container(_ => _.TransientTracking = TransientTracking.ExplicitReleaseMode);
            var child = root.CreateChildContainer();

            child.TransientTracking.ShouldNotBeSameAs(root.TransientTracking);
            child.TransientTracking.ShouldBeOfType<TrackingTransientCache>();

            var transient1 = root.GetInstance<DisposableGuy>();
            var transient2 = child.GetInstance<DisposableGuy>();

            child.Dispose();

            transient1.WasDisposed.ShouldBeFalse();
            transient2.WasDisposed.ShouldBeTrue();
        }

        public class Grandparent
        {
            public Grandparent(Parent parent)
            {
                Parent = parent;
            }

            public Parent Parent { get; set; }
        }

        public class Parent : IDisposable
        {
            public DisposableGuy Guy { get; set; }
            public bool WasDisposed;

            public void Dispose()
            {
                WasDisposed = true;
            }

            public Parent(DisposableGuy guy)
            {
                Guy = guy;
            }
        }

        public class DisposableGuy : IDisposable
        {
            public bool WasDisposed;

            public void Dispose()
            {
                WasDisposed = true;
            }
        }
    }
}