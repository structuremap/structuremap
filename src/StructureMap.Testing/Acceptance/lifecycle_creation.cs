using Shouldly;
using System;
using Xunit;

namespace StructureMap.Testing.Acceptance
{
    public class lifecycle_creation
    {
        // SAMPLE: SingletonThing-disposed-on-container-dispose
        public class DisposableSingleton : IDisposable
        {
            public void Dispose()
            {
                WasDisposed = true;
            }

            public bool WasDisposed { get; set; }
        }

        // ENDSAMPLE

        // SAMPLE: singleton-in-action
        [Fact]
        public void singletons_are_disposed_when_the_container_is_disposed()
        {
            var container = new Container(_ =>
            {
                _.ForSingletonOf<DisposableSingleton>();
            });

            // As a singleton-scoped object, every request for DisposableSingleton
            // will return the same object
            var singleton = container.GetInstance<DisposableSingleton>();
            singleton.ShouldBeSameAs(container.GetInstance<DisposableSingleton>());
            singleton.ShouldBeSameAs(container.GetInstance<DisposableSingleton>());
            singleton.ShouldBeSameAs(container.GetInstance<DisposableSingleton>());
            singleton.ShouldBeSameAs(container.GetInstance<DisposableSingleton>());

            singleton.WasDisposed.ShouldBeFalse();

            // now, dispose the Container
            container.Dispose();

            // the SingletonThing scoped object should be disposed
            singleton.WasDisposed.ShouldBeTrue();
        }

        // ENDSAMPLE

        [Fact]
        public void singletons_are_created_in_a_completely_separate_context_in_the_parent_container()
        {
            var container = new Container(x =>
            {
                x.ForSingletonOf<ISingletonThing>().Use<SingletonThing>();
                x.For<ITransient>().Use<Transient>();
            });

            Thing thing = null;
            using (var nested = container.GetNestedContainer())
            {
                thing = nested.GetInstance<Thing>();
            }

            // The transient object on Thing should be disposed
            thing.Transient.ShouldBeOfType<Transient>().WasDisposed.ShouldBeTrue();

            // SingletonThing should be created in the master scope and therefore,
            // not disposed by the nested container closing
            thing.SingletonThing.ShouldBeOfType<SingletonThing>()
                .Transient.ShouldBeOfType<Transient>()
                .WasDisposed.ShouldBeFalse();

            // The SingletonThing should not be sharing any children services with the transient
            thing.Transient.ShouldNotBeTheSameAs(thing.SingletonThing.ShouldBeOfType<SingletonThing>().Transient);
        }

        // SAMPLE: transient-are-shared-within-a-graph
        public interface IUnitOfWork
        {
        }

        public class DefaultUnitOfWork : IUnitOfWork
        {
        }

        public class Worker1
        {
            public IUnitOfWork Uow { get; set; }

            public Worker1(IUnitOfWork uow)
            {
                Uow = uow;
            }
        }

        public class Worker2
        {
            public IUnitOfWork Uow { get; set; }

            public Worker2(IUnitOfWork uow)
            {
                Uow = uow;
            }
        }

        public class WorkerCoordinator
        {
            public IUnitOfWork Uow { get; set; }
            public Worker1 Worker1 { get; set; }
            public Worker2 Worker2 { get; set; }

            public WorkerCoordinator(IUnitOfWork uow, Worker1 worker1, Worker2 worker2)
            {
                Uow = uow;
                Worker1 = worker1;
                Worker2 = worker2;
            }
        }

        [Fact]
        public void transient_scoped_Instance_is_built_once_per_resolution_to_the_Container()
        {
            var container = new Container(_ => { _.For<IUnitOfWork>().Use<DefaultUnitOfWork>(); });

            var cooridinator = container.GetInstance<WorkerCoordinator>();

            // The IUnitOfWork object instance is the same for
            // all the objects in the object graph that had
            // a constructor dependency on IUnitOfWork
            cooridinator.Uow
                .ShouldBeTheSameAs(cooridinator.Worker1.Uow);

            cooridinator.Uow
                .ShouldBeTheSameAs(cooridinator.Worker2.Uow);

            cooridinator.Worker1.Uow
                .ShouldBeTheSameAs(cooridinator.Worker2.Uow);
        }

        // ENDSAMPLE
    }

    public interface ISingletonThing
    {
    }

    public class SingletonThing : ISingletonThing
    {
        private readonly ITransient _transient;

        public SingletonThing(ITransient transient)
        {
            _transient = transient;
        }

        public ITransient Transient
        {
            get { return _transient; }
        }
    }

    public interface ITransient
    {
    }

    public class Transient : ITransient, IDisposable
    {
        public bool WasDisposed;

        public void Dispose()
        {
            WasDisposed = true;
        }
    }

    public interface IThing
    {
    }

    public class Thing : IThing
    {
        private readonly ITransient _transient;
        private readonly ISingletonThing _singletonThing;

        public Thing(ITransient transient, ISingletonThing singletonThing)
        {
            _transient = transient;
            _singletonThing = singletonThing;
        }

        public ITransient Transient
        {
            get { return _transient; }
        }

        public ISingletonThing SingletonThing
        {
            get { return _singletonThing; }
        }
    }
}