using System;
using NUnit.Framework;
using StructureMap.Configuration.DSL;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ContainerDisposalTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void disposing_a_nested_container_should_dispose_all_of_the_transient_objects_created_by_the_nested_container()
        {
            var container = new Container(x =>
            {
                x.For<I1>().Use<C1Yes>();
                x.For<I2>().Use<C2Yes>();
                x.For<I3>().AddInstances(o =>
                {
                    o.OfConcreteType<C3Yes>().WithName("1");
                    o.OfConcreteType<C3Yes>().WithName("2");
                });
            });

            var child = container.GetNestedContainer();

            var disposables = new Disposable[]
            {
                child.GetInstance<I1>().ShouldBeOfType<Disposable>(),
                child.GetInstance<I2>().ShouldBeOfType<Disposable>(),
                child.GetInstance<I3>("1").ShouldBeOfType<Disposable>(),
                child.GetInstance<I3>("2").ShouldBeOfType<Disposable>(),
            };

            child.Dispose();

            foreach (var disposable in disposables)
            {
                disposable.WasDisposed.ShouldBeTrue();
            }
        }

        [Test]
        public void disposing_a_nested_container_does_not_try_to_dispose_objects_created_by_the_parent()
        {
            var container = new Container(x =>
            {
                x.ForSingletonOf<I1>().Use<C1No>();
            });

            var child = container.GetNestedContainer();
            
            // Blows up if the Dispose() is called
            var notDisposable = child.GetInstance<I1>();

            child.Dispose();
        }

        [Test]
        public void should_dispose_objects_injected_into_the_container_1()
        {
            var container = new Container().GetNestedContainer();

            var disposable = new C1Yes();
            container.Inject<I1>(disposable);

            container.GetInstance<I1>().ShouldBeTheSameAs(disposable);

            container.Dispose();

            disposable.WasDisposed.ShouldBeTrue();
        }

        [Test]
        public void should_dispose_objects_injected_into_the_container_2()
        {
            var container = new Container().GetNestedContainer();

            var disposable = new C1Yes();
            container.Inject<I1>(disposable);

            container.Dispose();

            disposable.WasDisposed.ShouldBeTrue();
        }

        [Test]
        public void should_dispose_objects_injected_into_the_container_3()
        {
            var container = new Container().GetNestedContainer();

            var disposable = new C1Yes();
            container.Inject<I1>("blue", disposable);

            container.Dispose();

            disposable.WasDisposed.ShouldBeTrue();
        }


        [Test]
        public void should_dispose_objects_injected_into_the_container_4()
        {
            var container = new Container().GetNestedContainer();

            var disposable = new C1Yes();
            container.Inject(typeof(I1), disposable);

            container.Dispose();

            disposable.WasDisposed.ShouldBeTrue();
        }
    }

    public class Disposable : IDisposable
    {
        private bool _wasDisposed;

        public void Dispose()
        {
            if (_wasDisposed) Assert.Fail("This object should not be disposed twice");

            _wasDisposed = true;
        }

        public bool WasDisposed { get { return _wasDisposed; } }
    }

    public class NotDisposable : IDisposable
    {
        public void Dispose()
        {
            Assert.Fail("This object should not be disposed");
        }
    }

    public interface I1{}
    public interface I2{}
    public interface I3{}

    public class C1Yes : Disposable, I1{}
    public class C1No : NotDisposable, I1{}
    public class C2Yes : Disposable, I2{}
    public class C2No : Disposable, I2{}
    public class C3Yes : Disposable, I3{}
    public class C3No : Disposable, I3{}
}