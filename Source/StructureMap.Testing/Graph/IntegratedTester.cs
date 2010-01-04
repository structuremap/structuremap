using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Source;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class IntegratedTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            var graph = new PluginGraph();

            graph.Scan(x => x.Assembly("StructureMap.Testing.Widget"));

            DataMother.WriteDocument("IntegratedTest.XML");
            MementoSource source1 =
                new XmlFileMementoSource("IntegratedTest.XML", "GrandChildren", "GrandChild");

            MementoSource source2 = new XmlFileMementoSource("IntegratedTest.XML", "Children", "Child");
            MementoSource source3 = new XmlFileMementoSource("IntegratedTest.XML", "Parents", "Parent");

            graph.FindFamily(typeof (GrandChild)).AddMementoSource(source1);
            graph.FindFamily(typeof (Child)).AddMementoSource(source2);
            graph.FindFamily(typeof (Parent)).AddMementoSource(source3);

            container = new Container(graph);
        }

        #endregion

        private Container container;

        [Test]
        public void GetChildWithDefinedGrandChild()
        {
            var child = container.GetInstance(typeof (Child), "Tom") as Child;
            Assert.IsNotNull(child);
            Assert.AreEqual("Tom", child.Name);


            GrandChild grandChild = child.MyGrandChild;
            Assert.IsNotNull(grandChild);
            Assert.AreEqual(1984, grandChild.BirthYear, "Has correct BirthYear");
            Assert.IsTrue(grandChild is LeftieGrandChild, "Correct type?");
        }

        [Test, ExpectedException(typeof (StructureMapException))]
        public void GetChildWithInvalidGrandChild()
        {
            var child = container.GetInstance(typeof (Child), "Monte") as Child;
        }

        [Test]
        public void GetChildWithReferencedGrandChild()
        {
            var child = container.GetInstance(typeof (Child), "Marsha") as Child;
            Assert.IsNotNull(child);
            Assert.AreEqual("Marsha", child.Name);

            GrandChild grandChild = child.MyGrandChild;
            Assert.IsNotNull(grandChild);
            Assert.AreEqual(1972, grandChild.BirthYear, "Has correct BirthYear");
            Assert.AreEqual(true, grandChild.RightHanded, "Is Right-Handed?");
        }

        [Test]
        public void GetParentWithDefaultChildWhenChildIsNotReferenced()
        {
            container.Configure(x => x.For<Child>().TheDefault.Is.TheInstanceNamed("Marsha"));

            var parentOfMarsha = (Parent) container.GetInstance(typeof (Parent), "ImplicitChild");

            // This Parent does not have a Child specified, therefore return the default Child -- Marsha
            Assert.IsNotNull(parentOfMarsha);
            Assert.AreEqual("Marsha", parentOfMarsha.MyChild.Name);
        }


        [Test]
        public void GetParentWithDefinedChild()
        {
            var parent = container.GetInstance(typeof (Parent), "Jackie") as Parent;
            Assert.IsNotNull(parent);
            Assert.AreEqual(70, parent.Age, "Age = 70");
            Assert.AreEqual("Green", parent.EyeColor);

            Child child = parent.MyChild;
            Assert.IsNotNull(child);
            Assert.AreEqual("Elizabeth", child.Name);

            GrandChild grandChild = child.MyGrandChild;
            Assert.IsNotNull(grandChild);
            Assert.AreEqual(1992, grandChild.BirthYear, "Has correct BirthYear");
            Assert.IsTrue(grandChild is LeftieGrandChild, "Is a Leftie?");
        }

        [Test]
        public void GetParentWithReferencedChild()
        {
            var parent = container.GetInstance(typeof (Parent), "Jerry") as Parent;
            Assert.IsNotNull(parent);
            Assert.AreEqual(72, parent.Age, "Age = 72");
            Assert.AreEqual("Blue", parent.EyeColor);

            Child child = parent.MyChild;
            Assert.IsNotNull(child);
            Assert.AreEqual("Marsha", child.Name);

            GrandChild grandChild = child.MyGrandChild;
            Assert.IsNotNull(grandChild);
            Assert.AreEqual(1972, grandChild.BirthYear, "Has correct BirthYear");
            Assert.AreEqual(true, grandChild.RightHanded, "Is Right-Handed?");
        }

        [Test]
        public void GetParentWithReferenceToDefaultGrandChild()
        {
            container.Configure(x => x.For<GrandChild>().TheDefault.Is.TheInstanceNamed("Trevor"));
            var child = container.GetInstance(typeof (Child), "Jessica") as Child;
            Assert.IsNotNull(child);
            Assert.AreEqual("Jessica", child.Name);


            GrandChild grandChild = child.MyGrandChild;
            Assert.IsNotNull(grandChild);
            Assert.AreEqual(1979, grandChild.BirthYear, "Has correct BirthYear");
            Assert.IsTrue(grandChild is LeftieGrandChild, "Is a Leftie?");
        }
    }
}