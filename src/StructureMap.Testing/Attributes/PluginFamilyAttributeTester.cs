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

            Assert.IsInstanceOfType(interceptorType, family.Lifecycle);
        }

        [PluginFamily]
        public class TypeThatDoesNotHaveCustomMementoSource
        {
        }

        [PluginFamily(SourceType = typeof (CustomMementoSource))]
        public class TypeWithDesignatedMementoSource
        {
        }

        public class CustomMementoSource : MementoSource
        {
            public override string Description { get { throw new NotImplementedException(); } }

            protected override InstanceMemento[] fetchInternalMementos()
            {
                return new InstanceMemento[0];
            }

            protected override bool containsKey(string instanceKey)
            {
                throw new NotImplementedException();
            }

            protected override InstanceMemento retrieveMemento(string instanceKey)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void CreateMemoryMementoSourceWhenTheMementoSourceIsExplicitlyDefinedInAttribute()
        {
            var mocks = new MockRepository();
            var family = mocks.DynamicMock<IPluginFamily>();

            using (mocks.Record())
            {
                SetupResult.For(family.PluginType).Return(typeof (TypeWithDesignatedMementoSource));

                family.AddMementoSource(null);
                LastCall.Constraints(Is.TypeOf<CustomMementoSource>());
            }

            using (mocks.Playback())
            {
                PluginFamilyAttribute.ConfigureFamily(family);
            }
        }

        [Test]
        public void Do_not_add_a_memento_source_if_it_is_not_defined()
        {
            var mocks = new MockRepository();
            var family = mocks.DynamicMock<IPluginFamily>();

            using (mocks.Record())
            {
                SetupResult.For(family.PluginType).Return(typeof (TypeThatDoesNotHaveCustomMementoSource));

                family.AddMementoSource(null);
                LastCall.Repeat.Never();
            }

            using (mocks.Playback())
            {
                PluginFamilyAttribute.ConfigureFamily(family);
            }
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