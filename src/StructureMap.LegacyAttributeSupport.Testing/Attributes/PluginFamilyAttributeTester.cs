using System;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Attributes
{
    [TestFixture]
    public class PluginFamilyAttributeTester
    {
        private void assertScopeLeadsToInterceptor(string scope, Type interceptorType)
        {
            var att = new PluginFamilyAttribute();
            att.Scope = scope;

            var family = new PluginFamily(typeof (TypeThatDoesNotHaveCustomMementoSource));
            att.Configure(family);

            family.Lifecycle.ShouldBeOfType(interceptorType);
        }

        [PluginFamily]
        public class TypeThatDoesNotHaveCustomMementoSource
        {
        }

        [Test]
        public void ScopeToInterceptorTypes()
        {
            assertScopeLeadsToInterceptor(InstanceScope.HttpContext, typeof (HttpContextLifecycle));
            assertScopeLeadsToInterceptor(InstanceScope.Hybrid, typeof (HybridLifecycle));
            assertScopeLeadsToInterceptor(InstanceScope.Singleton, typeof (SingletonLifecycle));
            assertScopeLeadsToInterceptor(InstanceScope.ThreadLocal, typeof (ThreadLocalStorageLifecycle));
        }

        [Test]
        public void SettingTheScopeToSingletonSetsIsSingletonToTrue()
        {
            var att = new PluginFamilyAttribute();
            att.Scope = InstanceScope.Singleton;

            Assert.IsTrue(att.IsSingleton);
        }

        [Test]
        public void UseSingletonEqualsTrueSetsScopeToSingleton()
        {
            var att = new PluginFamilyAttribute();
            att.IsSingleton = true;

            Assert.AreEqual(InstanceScope.Singleton, att.Scope);
        }
    }
}