using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using StructureMap.Graph;
using StructureMap.Pipeline;
using Is = Rhino.Mocks.Constraints.Is;

namespace StructureMap.Testing.Attributes
{
    [TestFixture]
    public class PluginFamilyAttributeTester
    {
        private void assertScopeLeadsToInterceptor(InstanceScope scope, Type interceptorType)
        {
            var att = new PluginFamilyAttribute("something");
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
        public void PerRequest_DoesNot_call_SetScopeTo_on_family()
        {
            var att = new PluginFamilyAttribute("something");
            att.Scope = InstanceScope.PerRequest;

            var mocks = new MockRepository();
            var family = mocks.DynamicMock<IPluginFamily>();

            using (mocks.Record())
            {
                family.SetScopeTo(InstanceScope.PerRequest);
                LastCall.Repeat.Never();
            }

            using (mocks.Playback())
            {
                att.Configure(family);
            }
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