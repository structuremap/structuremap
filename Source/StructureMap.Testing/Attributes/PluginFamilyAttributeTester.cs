using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Attributes
{
    [TestFixture]
    public class PluginFamilyAttributeTester
    {
        private void assertScopeLeadsToInterceptor(InstanceScope scope, Type interceptorType)
        {
            PluginFamilyAttribute att = new PluginFamilyAttribute("something");
            att.Scope = scope;

            PluginFamily family = new PluginFamily(typeof (TypeThatDoesNotHaveCustomMementoSource));
            att.Configure(family);

            Assert.IsInstanceOfType(interceptorType, family.Policy);
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
            public override string Description
            {
                get { throw new NotImplementedException(); }
            }

            protected override InstanceMemento[] fetchInternalMementos()
            {
                return new InstanceMemento[0];
            }

            protected internal override bool containsKey(string instanceKey)
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
            MockRepository mocks = new MockRepository();
            IPluginFamily family = mocks.DynamicMock<IPluginFamily>();

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
            MockRepository mocks = new MockRepository();
            IPluginFamily family = mocks.DynamicMock<IPluginFamily>();

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
            PluginFamilyAttribute att = new PluginFamilyAttribute("something");
            att.Scope = InstanceScope.PerRequest;

            MockRepository mocks = new MockRepository();
            IPluginFamily family = mocks.DynamicMock<IPluginFamily>();

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
            assertScopeLeadsToInterceptor(InstanceScope.HttpContext, typeof (HttpContextBuildPolicy));
            assertScopeLeadsToInterceptor(InstanceScope.Hybrid, typeof (HybridBuildPolicy));
            assertScopeLeadsToInterceptor(InstanceScope.Singleton, typeof (SingletonPolicy));
            assertScopeLeadsToInterceptor(InstanceScope.ThreadLocal, typeof (ThreadLocalStoragePolicy));
        }

        [Test]
        public void SettingTheScopeToSingletonSetsIsSingletonToTrue()
        {
            PluginFamilyAttribute att = new PluginFamilyAttribute();
            att.Scope = InstanceScope.Singleton;

            Assert.IsTrue(att.IsSingleton);
        }

        [Test]
        public void UseSingletonEqualsTrueSetsScopeToSingleton()
        {
            PluginFamilyAttribute att = new PluginFamilyAttribute();
            att.IsSingleton = true;

            Assert.AreEqual(InstanceScope.Singleton, att.Scope);
        }
    }
}