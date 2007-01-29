using System;
using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Source;

namespace StructureMap.Testing.Attributes
{
    [TestFixture]
    public class PluginFamilyAttributeTester
    {
        [Test]
        public void CreatesAConfigInstanceMementoSourceByDefault()
        {
            PluginFamilyAttribute att = PluginFamilyAttribute.GetAttribute(typeof (Target1));

            MementoSource source = att.CreateSource(typeof (Target1));
            Assert.IsTrue(source is MemoryMementoSource);
        }

        [Test]
        public void CreateMemoryMementoSourceWhenTheMementoSourceIsExplicitlyDefinedInAttribute()
        {
            PluginFamilyAttribute att = PluginFamilyAttribute.GetAttribute(typeof (Target2));

            MementoSource source = att.CreateSource(typeof (Target2));
            Assert.IsTrue(source is CustomMementoSource);
        }

        [Test]
        public void ScopeIsPerRequestByDefault()
        {
            PluginFamilyAttribute att = new PluginFamilyAttribute();
            Assert.AreEqual(InstanceScope.PerRequest, att.Scope);
        }

        [Test]
        public void UseSingletonEqualsTrueSetsScopeToSingleton()
        {
            PluginFamilyAttribute att = new PluginFamilyAttribute();
            att.IsSingleton = true;

            Assert.AreEqual(InstanceScope.Singleton, att.Scope);
        }

        [Test]
        public void SettingTheScopeToSingletonSetsIsSingletonToTrue()
        {
            PluginFamilyAttribute att = new PluginFamilyAttribute();
            att.Scope = InstanceScope.Singleton;

            Assert.IsTrue(att.IsSingleton);
        }

        private void assertScopeLeadsToInterceptor(InstanceScope scope, Type interceptorType)
        {
            PluginFamilyAttribute att = new PluginFamilyAttribute("something");
            att.Scope = scope;

            PluginFamily family = att.BuildPluginFamily(typeof (Target1));
            Assert.AreEqual(1, family.InterceptionChain.Count);
            Type actualType = family.InterceptionChain[0].GetType();
            Assert.IsTrue(actualType.Equals(interceptorType));
        }

        [Test]
        public void PerRequestDoesNotHaveAnyInterceptors()
        {
            PluginFamilyAttribute att = new PluginFamilyAttribute("something");
            att.Scope = InstanceScope.PerRequest;

            PluginFamily family = att.BuildPluginFamily(typeof (Target1));
            Assert.AreEqual(0, family.InterceptionChain.Count);
        }

        [Test]
        public void ScopeToInterceptorTypes()
        {
            assertScopeLeadsToInterceptor(InstanceScope.HttpContext, typeof (HttpContextItemInterceptor));
            assertScopeLeadsToInterceptor(InstanceScope.Hybrid, typeof (HybridCacheInterceptor));
            assertScopeLeadsToInterceptor(InstanceScope.Singleton, typeof (SingletonInterceptor));
            assertScopeLeadsToInterceptor(InstanceScope.ThreadLocal, typeof (ThreadLocalStorageInterceptor));
        }

        [PluginFamily]
        public class Target1
        {
        }

        [PluginFamily(SourceType = typeof (CustomMementoSource))]
        public class Target2
        {
        }

        public class CustomMementoSource : MementoSource
        {
            protected override InstanceMemento[] fetchInternalMementos()
            {
                throw new NotImplementedException();
            }

            protected override bool containsKey(string instanceKey)
            {
                throw new NotImplementedException();
            }

            protected override InstanceMemento retrieveMemento(string instanceKey)
            {
                throw new NotImplementedException();
            }


            public override string Description
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}