using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Graph;
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
        public void Pass_in_arguments_as_dictionary()
        {
            ObjectFactory.Initialize(x => { x.ForRequestedType<IView>().TheDefaultIsConcreteType<View>(); });

            var theNode = new Node();
            var theTrade = new Trade();

            var args = new ExplicitArguments();
            args.Set(theNode);
            args.SetArg("trade", theTrade);

            var command = ObjectFactory.GetInstance<Command>(args);

            Assert.IsInstanceOfType(typeof (View), command.View);
            Assert.AreSame(theNode, command.Node);
            Assert.AreSame(theTrade, command.Trade);
        }

        [Test]
        public void SmokeTestGetAllInstances()
        {
            IList list = ObjectFactory.GetAllInstances(typeof (GrandChild));
            Assert.IsTrue(list.Count > 0);
        }

        [Test]
        public void SmokeTestGetAllInstancesGeneric()
        {
            IList<GrandChild> list = ObjectFactory.GetAllInstances<GrandChild>();
            Assert.IsTrue(list.Count > 0);
        }

        [Test]
        public void SmokeTestModelAccess()
        {
            ObjectFactory.Model.PluginTypes.Count().ShouldBeGreaterThan(0);
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
            var defaultGrandChild = (GrandChild) ObjectFactory.GetInstance(typeof (GrandChild));
            var todd = (GrandChild) ObjectFactory.GetNamedInstance(typeof (GrandChild), "Todd");
            Assert.AreEqual(todd.BirthYear, defaultGrandChild.BirthYear);
        }

        [Test]
        public void TheDefaultProfileIsSetWhileUsingGenericGetInstance()
        {
            ObjectFactory.ResetDefaults();

            // The default GrandChild is set within the default profile
            var todd = ObjectFactory.GetNamedInstance<GrandChild>("Todd");
            var defaultGrandChild = ObjectFactory.GetInstance<GrandChild>();

            Assert.AreEqual(todd.BirthYear, defaultGrandChild.BirthYear);
        }
    }
}