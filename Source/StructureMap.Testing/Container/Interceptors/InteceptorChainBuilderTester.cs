using System;
using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Interceptors;

namespace StructureMap.Testing.Container.Interceptors
{
    [TestFixture]
    public class InteceptorChainBuilderTester
    {
        private InterceptorChainBuilder builder = new InterceptorChainBuilder();

        [Test]
        public void PerInstanceMeansNoInterceptors()
        {
            InterceptionChain chain = builder.Build(InstanceScope.PerRequest);
            Assert.AreEqual(0, chain.Count);
        }

        private void assertScopeLeadsToInterceptor(InstanceScope scope, Type interceptorType)
        {
            InterceptionChain chain = builder.Build(scope);
            Assert.AreEqual(1, chain.Count);
            Type actualType = chain[0].GetType();
            Assert.IsTrue(actualType.Equals(interceptorType));
        }


        [Test]
        public void ScopeToInterceptorTypes()
        {
            assertScopeLeadsToInterceptor(InstanceScope.HttpContext, typeof (HttpContextItemInterceptor));
            assertScopeLeadsToInterceptor(InstanceScope.Hybrid, typeof (HybridCacheInterceptor));
            assertScopeLeadsToInterceptor(InstanceScope.Singleton, typeof (SingletonInterceptor));
            assertScopeLeadsToInterceptor(InstanceScope.ThreadLocal, typeof (ThreadLocalStorageInterceptor));
        }
    }
}