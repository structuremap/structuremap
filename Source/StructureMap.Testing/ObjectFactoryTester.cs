using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Xml;
using NUnit.Framework;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;
using IList=System.Collections.IList;

namespace StructureMap.Testing
{
    [TestFixture]
    public class ObjectFactoryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            DataMother.RestoreStructureMapConfig();
            ObjectFactory.Initialize(x => x.UseDefaultStructureMapConfigFile = true);
        }

        #endregion

        [Test]
        public void SmokeTestGetAllInstances()
        {
            IList list = ObjectFactory.GetAllInstances(typeof (GrandChild));
            Assert.IsTrue(list.Count > 0);
        }

        [Test]
        public void SmokeTestModelAccess()
        {
            ObjectFactory.Model.PluginTypes.Count().ShouldBeGreaterThan(0);
        }

        [Test]
        public void SmokeTestGetAllInstancesGeneric()
        {
            IList<GrandChild> list = ObjectFactory.GetAllInstances<GrandChild>();
            Assert.IsTrue(list.Count > 0);
        }

        [Test]
        public void SmokeTestWhatDoIHave()
        {
            string message = ObjectFactory.WhatDoIHave();
            Debug.WriteLine(message);
        }

        [Test]
        public void TheDefaultProfileIsSet()
        {
            ObjectFactory.ResetDefaults();

            // The default GrandChild is set within the default profile
            GrandChild defaultGrandChild = (GrandChild) ObjectFactory.GetInstance(typeof (GrandChild));
            GrandChild todd = (GrandChild) ObjectFactory.GetNamedInstance(typeof (GrandChild), "Todd");
            Assert.AreEqual(todd.BirthYear, defaultGrandChild.BirthYear);
        }

        [Test]
        public void TheDefaultProfileIsSetWhileUsingGenericGetInstance()
        {
            ObjectFactory.ResetDefaults();

            // The default GrandChild is set within the default profile
            GrandChild todd = ObjectFactory.GetNamedInstance<GrandChild>("Todd");
            GrandChild defaultGrandChild = ObjectFactory.GetInstance<GrandChild>();

            Assert.AreEqual(todd.BirthYear, defaultGrandChild.BirthYear);
        }
    }
}