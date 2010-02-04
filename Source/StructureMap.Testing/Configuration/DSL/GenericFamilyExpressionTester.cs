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
            var manager =
                new Container(
                    r => r.ForRequestedType(typeof (ITarget)).AddConcreteType(typeof (Target1)));


            Assert.IsInstanceOfType(typeof (Target1), manager.GetAllInstances<ITarget>()[0]);
        }

        [Test]
        public void Add_concrete_type_with_name()
        {
            var manager = new Container(r =>
            {
                r.ForRequestedType(typeof (ITarget)).AddConcreteType(typeof (Target1), "1");
                r.ForRequestedType(typeof (ITarget)).AddConcreteType(typeof (Target2), "2");
                r.ForRequestedType(typeof (ITarget)).AddConcreteType(typeof (Target3), "3");
            });


            Assert.IsInstanceOfType(typeof (Target1), manager.GetInstance<ITarget>("1"));
            Assert.IsInstanceOfType(typeof (Target2), manager.GetInstance<ITarget>("2"));
            Assert.IsInstanceOfType(typeof (Target3), manager.GetInstance<ITarget>("3"));
        }

        [Test]
        public void Add_default_by_concrete_type()
        {
            var manager =
                new Container(
                    r => r.ForRequestedType(typeof (ITarget)).TheDefaultIsConcreteType(typeof (Target3)));

            Assert.IsInstanceOfType(typeof (Target3), manager.GetInstance<ITarget>());
        }

        [Test]
        public void Add_default_instance()
        {
            var container =
                new Container(r => { r.ForRequestedType(typeof (ITarget)).TheDefaultIsConcreteType(typeof (Target2)); });

            container.GetInstance<ITarget>().ShouldBeOfType<Target2>();
        }


        [Test]
        public void Add_instance_directly()
        {
            var container = new Container(r =>
            {
                r.For<ITarget>().Add<Target2>();
            });

            Assert.IsInstanceOfType(typeof (Target2), container.GetAllInstances<ITarget>()[0]);
        }

        [Test]
        public void Enrichment()
        {
            var container = new Container(r =>
            {
                r.ForRequestedType(typeof (ITarget)).EnrichWith(raw => new WrappedTarget((ITarget) raw))
                    .TheDefaultIsConcreteType(typeof (Target1));
            });

            var target = (WrappedTarget) container.GetInstance<ITarget>();
            Assert.IsInstanceOfType(typeof (Target1), target.Inner);
        }

        [Test]
        public void On_creation()
        {
            ITarget created = null;

            var container = new Container(r =>
            {
                r.ForRequestedType(typeof (ITarget)).OnCreation(raw => created = (ITarget) raw)
                    .TheDefaultIsConcreteType(typeof (Target3));
            });

            container.GetInstance<ITarget>().ShouldBeOfType<Target3>();
        }

        [Test]
        public void Set_caching()
        {
            var registry = new Registry();
            registry.ForRequestedType(typeof (ITarget)).CacheBy(InstanceScope.ThreadLocal);
            PluginGraph graph = registry.Build();

            graph.FindFamily(typeof (ITarget)).Lifecycle.ShouldBeOfType<ThreadLocalStorageLifecycle>();
        }
    }
}