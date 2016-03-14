using System;
using Xunit;

namespace StructureMap.Testing.Acceptance
{
    public class container_scoping
    {
        public class Disposable : IDisposable
        {
            private bool _wasDisposed;

            public bool WasDisposed
            {
                get { return _wasDisposed; }
            }

            public void Dispose()
            {
                Assert.False(_wasDisposed, "This object should not be disposed twice");

                _wasDisposed = true;
            }
        }

        // SAMPLE: container-scoped-in-action
        [Fact]
        public void container_scoping_with_root_child_and_nested_container()
        {
            var container = new Container(_ =>
            {
                _.ForConcreteType<Disposable>().Configure.ContainerScoped();
            });

            var child = container.CreateChildContainer();

            var nested = container.GetNestedContainer();

            // Always the same object when requested from the root container
            var mainDisposable = container.GetInstance<Disposable>();
            mainDisposable
                .ShouldBeTheSameAs(container.GetInstance<Disposable>());

            // Always the same object when requested from a child container
            var childDisposable = child.GetInstance<Disposable>();
            childDisposable
                .ShouldBeTheSameAs(child.GetInstance<Disposable>());

            // Always the same object when requested from a nested container
            var nestedDisposable = nested.GetInstance<Disposable>();
            nestedDisposable
                .ShouldBeTheSameAs(nested.GetInstance<Disposable>());

            // It should be a different object instance for
            // all three containers
            mainDisposable
                .ShouldNotBeTheSameAs(childDisposable);

            mainDisposable
                .ShouldNotBeTheSameAs(nestedDisposable);

            childDisposable
                .ShouldNotBeTheSameAs(nestedDisposable);

            // When the nested container is disposed,
            // it should dispose all the container scoped objects,
            // but not impact the other containers
            nested.Dispose();
            nestedDisposable.WasDisposed.ShouldBeTrue();
            childDisposable.WasDisposed.ShouldBeFalse();
            mainDisposable.WasDisposed.ShouldBeFalse();

            // Same for the child container
            child.Dispose();
            childDisposable.WasDisposed.ShouldBeTrue();
            mainDisposable.WasDisposed.ShouldBeFalse();

            // Same for the main container
            container.Dispose();
            mainDisposable.WasDisposed.ShouldBeTrue();
        }

        // ENDSAMPLE
    }
}