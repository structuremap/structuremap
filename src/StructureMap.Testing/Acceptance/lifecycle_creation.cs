using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace StructureMap.Testing.Acceptance
{




    [TestFixture]
    public class lifecycle_creation
    {
        // SAMPLE: singleton-disposed-on-container-dispose
        public class DisposableSingleton : IDisposable
        {
            public void Dispose()
            {
                WasDisposed = true;
            }

            public bool WasDisposed { get; set; }
        }

        [Test]
        public void singletons_are_disposed_when_the_container_is_disposed()
        {
            var container = new Container(_ => {
                _.ForSingletonOf<DisposableSingleton>();
            });

            var singleton = container.GetInstance<DisposableSingleton>();
            singleton.WasDisposed.ShouldBeFalse();

            // now, dispose the Container
            container.Dispose();

            // the singleton scoped object should be disposed
            singleton.WasDisposed.ShouldBeTrue();
        }
        // ENDSAMPLE

        [Test]
        public void singletons_are_created_in_a_completely_separate_context_in_the_parent_container()
        {
            var container = new Container(x => {
                x.ForSingletonOf<ISingleton>().Use<Singleton>();
                x.For<ITransient>().Use<Transient>();
            });

            Thing thing = null;
            using (var nested = container.GetNestedContainer())
            {
                thing = nested.GetInstance<Thing>();
            }

            // The transient object on Thing should be disposed
            thing.Transient.ShouldBeOfType<Transient>().WasDisposed.ShouldBeTrue();

            // Singleton should be created in the master scope and therefore,
            // not disposed by the nested container closing
            thing.Singleton.ShouldBeOfType<Singleton>()
                .Transient.ShouldBeOfType<Transient>()
                .WasDisposed.ShouldBeFalse();

            // The singleton should not be sharing any children services with the transient
            thing.Transient.ShouldNotBeTheSameAs(thing.Singleton.ShouldBeOfType<Singleton>().Transient);
        }

        // SAMPLE: transient-are-shared-within-a-graph
        public interface IUnitOfWork{}
        public class DefaultUnitOfWork : IUnitOfWork{}

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

        [Test]
        public void transient_scoped_Instance_is_built_once_per_resolution_to_the_Container()
        {
            var container = new Container(_ => {
                _.For<IUnitOfWork>().Use<DefaultUnitOfWork>();
            });

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

    public interface ISingleton
    {
    }

    public class Singleton : ISingleton
    {
        private readonly ITransient _transient;

        public Singleton(ITransient transient)
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
        private readonly ISingleton _singleton;

        public Thing(ITransient transient, ISingleton singleton)
        {
            _transient = transient;
            _singleton = singleton;
        }

        public ITransient Transient
        {
            get { return _transient; }
        }

        public ISingleton Singleton
        {
            get { return _singleton; }
        }
    }
}