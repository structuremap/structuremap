using System;
using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class GenericFamilyExpressionTester : RegistryExpressions
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

            public ITarget Inner
            {
                get { return _inner; }
            }
        }

        [Test]
        public void Add_concrete_type()
        {
            InstanceManager manager = new InstanceManager(delegate(Registry r)
            {
                r.ForRequestedType(typeof(ITarget)).AddConcreteType(typeof(Target1));
            });


            Assert.IsInstanceOfType(typeof(Target1), manager.GetAllInstances<ITarget>()[0]);
        }

        [Test]
        public void Add_concrete_type_with_name()
        {
            InstanceManager manager = new InstanceManager(delegate(Registry r)
            {
                r.ForRequestedType(typeof(ITarget)).AddConcreteType(typeof(Target1), "1");
                r.ForRequestedType(typeof(ITarget)).AddConcreteType(typeof(Target2), "2");
                r.ForRequestedType(typeof(ITarget)).AddConcreteType(typeof(Target3), "3");
            });


            Assert.IsInstanceOfType(typeof(Target1), manager.CreateInstance<ITarget>("1"));
            Assert.IsInstanceOfType(typeof(Target2), manager.CreateInstance<ITarget>("2"));
            Assert.IsInstanceOfType(typeof(Target3), manager.CreateInstance<ITarget>("3"));
        }

        [Test]
        public void Add_default_by_concrete_type()
        {
            InstanceManager manager = new InstanceManager(delegate(Registry r)
            {
                r.ForRequestedType(typeof (ITarget)).TheDefaultIsConcreteType(typeof (Target3));
            });

            Assert.IsInstanceOfType(typeof(Target3), manager.CreateInstance<ITarget>());
        }

        [Test]
        public void Add_default_instance()
        {
            InstanceManager manager = new InstanceManager(delegate(Registry r)
            {
                r.ForRequestedType(typeof(ITarget)).TheDefaultIs(Instance<Target2>());
            });

            Assert.IsInstanceOfType(typeof(Target2), manager.CreateInstance<ITarget>());
        }

        [Test]
        public void Add_instance_by_lambda()
        {
            InstanceManager manager = new InstanceManager(delegate(Registry r)
            {
                r.ForRequestedType(typeof(ITarget)).TheDefaultIs(delegate() { return new Target1(); });
            });

            Assert.IsInstanceOfType(typeof(Target1), manager.CreateInstance<ITarget>());
        }

        [Test]
        public void Add_instance_directly()
        {
            InstanceManager manager = new InstanceManager(delegate(Registry r)
            {
                r.ForRequestedType(typeof (ITarget)).AddInstance(Instance<Target2>());
            });


            Assert.IsInstanceOfType(typeof(Target2), manager.GetAllInstances<ITarget>()[0]);
        }

        [Test]
        public void Enrichment()
        {
            InstanceManager manager = new InstanceManager(delegate(Registry r)
            {
                r.ForRequestedType(typeof(ITarget))
                    .TheDefaultIsConcreteType(typeof(Target1))
                    .EnrichWith(delegate(object raw){ return new WrappedTarget((ITarget) raw);});
            });

            WrappedTarget target = (WrappedTarget) manager.CreateInstance<ITarget>();
            Assert.IsInstanceOfType(typeof(Target1), target.Inner);
        }

        [Test]
        public void Intercept_construction_with()
        {
            Registry registry = new Registry();
            TestingBuildPolicy policy = new TestingBuildPolicy();
            registry.ForRequestedType(typeof (ITarget)).InterceptConstructionWith(policy);
            PluginGraph graph = registry.Build();

            Assert.AreSame(policy, graph.FindFamily(typeof(ITarget)).Policy);
        }

        public class TestingBuildPolicy : IBuildInterceptor
        {
            public IBuildPolicy InnerPolicy
            {
                get { throw new NotImplementedException(); }
                set {  }
            }

            public object Build(IBuildSession buildSession, Type pluginType, Instance instance)
            {
                throw new NotImplementedException();
            }

            public IBuildPolicy Clone()
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void On_creation()
        {
            ITarget created = null;

            InstanceManager manager = new InstanceManager(delegate(Registry r)
            {
                r.ForRequestedType(typeof (ITarget))
                    .TheDefaultIsConcreteType(typeof (Target3))
                    .OnCreation(delegate(object raw) { created = (ITarget) raw; });
            });

            manager.CreateInstance<ITarget>();

            Assert.IsInstanceOfType(typeof(Target3), created);
        }

        [Test]
        public void Set_caching()
        {
            Registry registry = new Registry();
            registry.ForRequestedType(typeof (ITarget)).CacheBy(InstanceScope.ThreadLocal);
            PluginGraph graph = registry.Build();

            Assert.IsInstanceOfType(typeof(ThreadLocalStoragePolicy), graph.FindFamily(typeof(ITarget)).Policy);
        }
    }
}