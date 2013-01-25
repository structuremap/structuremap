using System;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class GenericFamilyExpressionTester
    {
        public interface ITarget
        {
        }

        public class Target1 : ITarget
        {
        }

        public class Target2 : ITarget
        {
        }

        public class Target3 : ITarget
        {
        }

        public class WrappedTarget : ITarget
        {
            private readonly ITarget _inner;

            public WrappedTarget(ITarget target)
            {
                _inner = target;
            }

            public ITarget Inner { get { return _inner; } }
        }


        public interface IRepository<T>
        {
            void Save(T subject);
        }

        public class OnlineRepository<T> : IRepository<T>
        {
            #region IRepository<T> Members

            public void Save(T subject)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        public class OfflineRepository<T> : IRepository<T>
        {
            #region IRepository<T> Members

            public void Save(T subject)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        public class Invoice
        {
        }

        [Test]
        public void Add_concrete_type()
        {
            var container =
                new Container(
                    r => r.For(typeof (ITarget)).AddConcreteType(typeof (Target1)));


            container.GetAllInstances<ITarget>()[0].ShouldBeOfType<Target1>();
        }

        [Test]
        public void Add_concrete_type_with_name()
        {
            var container = new Container(r =>
            {
                r.For(typeof (ITarget)).AddConcreteType(typeof (Target1), "1");
                r.For(typeof (ITarget)).AddConcreteType(typeof (Target2), "2");
                r.For(typeof (ITarget)).AddConcreteType(typeof (Target3), "3");
            });

            container.GetInstance<ITarget>("1").ShouldBeOfType<Target1>();
            container.GetInstance<ITarget>("2").ShouldBeOfType<Target2>();
            container.GetInstance<ITarget>("3").ShouldBeOfType<Target3>();
        }

        [Test]
        public void Add_default_by_concrete_type()
        {
            var container =
                new Container(
                    r => r.For(typeof (ITarget)).Use(typeof (Target3)));

            container.GetInstance<ITarget>().ShouldBeOfType<Target3>();
        }

        [Test]
        public void Add_default_instance()
        {
            var container =
                new Container(r => { r.For(typeof (ITarget)).Use(typeof (Target2)); });

            container.GetInstance<ITarget>().ShouldBeOfType<Target2>();
        }


        [Test]
        public void Add_instance_directly()
        {
            var container = new Container(r =>
            {
                r.For<ITarget>().Add<Target2>();
            });

            container.GetAllInstances<ITarget>()[0].ShouldBeOfType<Target2>();
        }

        [Test]
        public void Enrichment()
        {
            var container = new Container(r =>
            {
                r.For(typeof (ITarget)).EnrichAllWith(raw => new WrappedTarget((ITarget) raw))
                    .Use(typeof (Target1));
            });

            var target = (WrappedTarget) container.GetInstance<ITarget>();

            target.Inner.ShouldBeOfType<Target1>();
        }

        [Test]
        public void On_creation()
        {
            ITarget created = null;

            var container = new Container(r =>
            {
                r.For(typeof (ITarget)).OnCreationForAll(raw => created = (ITarget) raw)
                    .Use(typeof (Target3));
            });

            container.GetInstance<ITarget>().ShouldBeOfType<Target3>();
        }

        [Test]
        public void Set_caching()
        {
            var registry = new Registry();
            registry.For(typeof(ITarget), InstanceScope.ThreadLocal);
            PluginGraph graph = registry.Build();

            graph.FindFamily(typeof (ITarget)).Lifecycle.ShouldBeOfType<ThreadLocalStorageLifecycle>();
        }
    }
}