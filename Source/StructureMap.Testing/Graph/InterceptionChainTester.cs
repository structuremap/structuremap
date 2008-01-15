using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class InterceptionChainTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _singleton1 = new SingletonInterceptor();
            _singleton2 = new SingletonInterceptor();

            _chain = new InterceptionChain();
            _chain.AddInterceptor(_singleton1);
            _chain.AddInterceptor(_singleton2);
        }

        #endregion

        private InterceptionChain _chain;
        private SingletonInterceptor _singleton1;
        private SingletonInterceptor _singleton2;

        [Test]
        public void CreateInterceptionChainWithNoIntercepters()
        {
            InterceptionChain chain = new InterceptionChain();
            PluginFamily family = ObjectMother.GetPluginFamily(typeof (Rule));
            InstanceFactory factory = new InstanceFactory(family, true);

            IInstanceFactory wrappedFactory = chain.WrapInstanceFactory(factory);

            Assert.AreSame(factory, wrappedFactory);
        }

        [Test]
        public void CreateInterceptionChainWithOneIntercepter()
        {
            InterceptionChain chain = new InterceptionChain();
            chain.AddInterceptor(_singleton1);

            PluginFamily family = ObjectMother.GetPluginFamily(typeof (Rule));
            InstanceFactory factory = new InstanceFactory(family, true);

            IInstanceFactory wrappedFactory = chain.WrapInstanceFactory(factory);
            Assert.AreSame(_singleton1, wrappedFactory);
            Assert.AreSame(factory, _singleton1.InnerInstanceFactory);
        }

        [Test]
        public void CreateInterceptionChainWithTwoInterceptors()
        {
            PluginFamily family = ObjectMother.GetPluginFamily(typeof (Rule));
            InstanceFactory factory = new InstanceFactory(family, true);

            IInstanceFactory wrappedFactory = _chain.WrapInstanceFactory(factory);

            Assert.IsNotNull(wrappedFactory);

            // Cast exception if wrappedFactory is not a Singleton
            SingletonInterceptor singletonWrapper = (SingletonInterceptor) wrappedFactory;

            Assert.AreSame(_singleton1, singletonWrapper);
            Assert.AreSame(_singleton2, _singleton1.InnerInstanceFactory);
            Assert.AreSame(factory, _singleton2.InnerInstanceFactory);
        }

        [Test]
        public void tryIt()
        {
            ObjectMother.Reset();
        }
    }
}