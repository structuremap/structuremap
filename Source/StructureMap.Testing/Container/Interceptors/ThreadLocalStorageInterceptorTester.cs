using System.Threading;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Container.Interceptors
{
    [TestFixture]
    public class ThreadLocalStorageInterceptorTester
    {
        private ThreadLocalStorageInterceptor _interceptor;
        private ColorRule _rule1;
        private ColorRule _rule2;
        private ColorRule _rule3;


        [SetUp]
        public void SetUp()
        {
            PluginFamily family = ObjectMother.GetPluginFamily(typeof (Rule));
            InstanceFactory factory = new InstanceFactory(family, true);

            _interceptor = new ThreadLocalStorageInterceptor();
            _interceptor.InnerInstanceFactory = factory;
        }

        private void findRule1()
        {
            _rule1 = (ColorRule) _interceptor.GetInstance();

            ColorRule rule = (ColorRule) _interceptor.GetInstance();
            Assert.AreSame(_rule1, rule);
        }

        private void findRule2()
        {
            _rule2 = (ColorRule) _interceptor.GetInstance();

            ColorRule rule = (ColorRule) _interceptor.GetInstance();
            Assert.AreSame(_rule2, rule);
        }

        private void findRule3()
        {
            _rule3 = (ColorRule) _interceptor.GetInstance();

            ColorRule rule = (ColorRule) _interceptor.GetInstance();
            Assert.AreSame(_rule3, rule);

            rule = (ColorRule) _interceptor.GetInstance();
            Assert.AreSame(_rule3, rule);

            rule = (ColorRule) _interceptor.GetInstance();
            Assert.AreSame(_rule3, rule);

            rule = (ColorRule) _interceptor.GetInstance();
            Assert.AreSame(_rule3, rule);
        }

        [Test]
        public void FindUniqueInstancePerThread()
        {
            Thread t1 = new Thread(new ThreadStart(findRule1));
            Thread t2 = new Thread(new ThreadStart(findRule2));
            Thread t3 = new Thread(new ThreadStart(findRule3));

            t1.Start();
            t2.Start();
            t3.Start();

            t1.Join();
            t2.Join();
            t3.Join();

            Assert.IsTrue(_rule1.ID != _rule2.ID);
            Assert.IsTrue(_rule1.ID != _rule3.ID);
            Assert.IsTrue(_rule2.ID != _rule3.ID);
        }
    }
}