using System.Threading;
using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ThreadLocalStoragePolicyTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _policy = new ThreadLocalStoragePolicy();
            _instance = new ConstructorInstance<ColorRule>(() => new ColorRule("Red")).WithName("Red");
        }

        #endregion

        private ThreadLocalStoragePolicy _policy;
        private ColorRule _rule1;
        private ColorRule _rule2;
        private ColorRule _rule3;
        private ConstructorInstance<ColorRule> _instance;


        private void findRule1()
        {
            _rule1 = (ColorRule) _policy.Build(new StubBuildSession(), typeof (IService), _instance);

            ColorRule rule = (ColorRule) _policy.Build(new StubBuildSession(), typeof (IService), _instance);
            Assert.AreSame(_rule1, rule);
        }

        private void findRule2()
        {
            _rule2 = (ColorRule) _policy.Build(new StubBuildSession(), typeof (IService), _instance);

            ColorRule rule = (ColorRule) _policy.Build(new StubBuildSession(), typeof (IService), _instance);
            Assert.AreSame(_rule2, rule);
        }

        private void findRule3()
        {
            _rule3 = (ColorRule) _policy.Build(new StubBuildSession(), typeof (IService), _instance);

            ColorRule rule = (ColorRule) _policy.Build(new StubBuildSession(), typeof (IService), _instance);
            Assert.AreSame(_rule3, rule);

            rule = (ColorRule) _policy.Build(new StubBuildSession(), typeof (IService), _instance);
            Assert.AreSame(_rule3, rule);

            rule = (ColorRule) _policy.Build(new StubBuildSession(), typeof (IService), _instance);
            Assert.AreSame(_rule3, rule);

            rule = (ColorRule) _policy.Build(new StubBuildSession(), typeof (IService), _instance);
            Assert.AreSame(_rule3, rule);
        }

        [Test]
        public void FindUniqueInstancePerThread()
        {
            Thread t1 = new Thread(findRule1);
            Thread t2 = new Thread(findRule2);
            Thread t3 = new Thread(findRule3);

            t1.Start();
            t2.Start();
            t3.Start();

            t1.Join();
            t2.Join();
            t3.Join();

            Assert.AreNotSame(_rule1, _rule2);
            Assert.AreNotSame(_rule1, _rule3);
            Assert.AreNotSame(_rule2, _rule3);
            Assert.IsTrue(_rule1.ID != _rule2.ID);
            Assert.IsTrue(_rule1.ID != _rule3.ID);
            Assert.IsTrue(_rule2.ID != _rule3.ID);
        }
    }
}