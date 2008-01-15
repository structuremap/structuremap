using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Container.Interceptors
{
    [TestFixture]
    public class SingletonInterceptorTester
    {
        // "Red", "Blue", "Bigger" are the possible rule choices

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            PluginFamily family = ObjectMother.GetPluginFamily(typeof (Rule));
            InstanceFactory factory = new InstanceFactory(family, true);

            _interceptor = new SingletonInterceptor();
            _interceptor.InnerInstanceFactory = factory;
        }

        #endregion

        private SingletonInterceptor _interceptor;

        [Test]
        public void CanSetDefaultAndGetTheDefaultInstance()
        {
            _interceptor.SetDefault("Red");
            Assert.AreEqual("Red", _interceptor.DefaultInstanceKey);

            ColorRule rule1 = (ColorRule) _interceptor.GetInstance();
            Assert.AreEqual("Red", rule1.Color);

            // Fetch the rule 2 more times.  Verify that it is the same object
            ColorRule rule2 = (ColorRule) _interceptor.GetInstance();
            ColorRule rule3 = (ColorRule) _interceptor.GetInstance();

            Assert.AreSame(rule1, rule2);
            Assert.AreSame(rule1, rule3);
        }

        [Test]
        public void GetsTheSameNamedInstance()
        {
            ColorRule rule1 = (ColorRule) _interceptor.GetInstance("Blue");

            // Fetch the rule 2 more times.  Verify that it is the same object
            ColorRule rule2 = (ColorRule) _interceptor.GetInstance("Blue");
            ColorRule rule3 = (ColorRule) _interceptor.GetInstance("Blue");

            Assert.AreSame(rule1, rule2);
            Assert.AreSame(rule1, rule3);
        }
    }
}