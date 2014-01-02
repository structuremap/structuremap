using System;
using NUnit.Framework;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class lifecycle_creation
    {
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