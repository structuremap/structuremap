using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;
using IList=System.Collections.IList;

namespace StructureMap.Testing.Container
{
    [TestFixture]
    public class FullStackFacadeTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            StructureMapConfiguration.ResetAll();
            

            DataMother.WriteDocument("SampleConfig.xml");
            DataMother.WriteDocument("FullTesting.XML");
            
            ObjectFactory.Reset();
        }

        #endregion

        [Test]
        public void BuildAllInstances()
        {
            DataMother.WriteDocument("FullTesting.XML");

            PluginGraph pluginGraph = DataMother.GetDiagnosticPluginGraph("SampleConfig.xml");

            InstanceManager manager = new InstanceManager(pluginGraph);

            IList list = manager.GetAllInstances(typeof (IWidget));

            Assert.AreEqual(7, list.Count);

            foreach (object target in list)
            {
                Assert.IsTrue(target is IWidget);
            }
        }

        [Test]
        public void ChangeDefault()
        {
            ObjectFactory.SetDefaultInstanceName(typeof (IWidget), "Green");
            ColorWidget green = (ColorWidget) ObjectFactory.GetInstance(typeof (IWidget));
            Assert.IsNotNull(green);
            Assert.AreEqual("Green", green.Color);
        }

        [Test]
        public void ChangeDefaultWithGenericMethod()
        {
            ObjectFactory.SetDefaultInstanceName<IWidget>("Green");
            ColorWidget green = (ColorWidget) ObjectFactory.GetInstance<IWidget>();
            Assert.IsNotNull(green);
            Assert.AreEqual("Green", green.Color);
        }

        [Test]
        public void FillDependenc1ies2()
        {
            FilledTarget target = ObjectFactory.FillDependencies<FilledTarget>();
            Assert.IsNotNull(target.Gateway);
            Assert.IsNotNull(target.Rule);
        }

        [Test]
        public void FillDependencies()
        {
            FilledTarget target = (FilledTarget) ObjectFactory.FillDependencies(typeof (FilledTarget));
            Assert.IsNotNull(target.Gateway);
            Assert.IsNotNull(target.Rule);
        }

        [Test]
        public void GetChildWithDefinedGrandChild()
        {
            Child child = ObjectFactory.GetNamedInstance(typeof (Child), "Tom") as Child;
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
            Child child = ObjectFactory.GetNamedInstance(typeof (Child), "Monte") as Child;
        }

        [Test]
        public void GetChildWithReferencedGrandChild()
        {
            Child child = ObjectFactory.GetNamedInstance(typeof (Child), "Marsha") as Child;
            Assert.IsNotNull(child);
            Assert.AreEqual("Marsha", child.Name);

            GrandChild grandChild = child.MyGrandChild;
            Assert.IsNotNull(grandChild);
            Assert.AreEqual(1972, grandChild.BirthYear, "Has correct BirthYear");
            Assert.AreEqual(true, grandChild.RightHanded, "Is Right-Handed?");
        }

        [Test]
        public void GetDefaultWidget()
        {
            // "Red" is marked as the default in the configuration
            ColorWidget red = (ColorWidget) ObjectFactory.GetInstance(typeof (IWidget));
            Assert.IsNotNull(red);
            Assert.AreEqual("Red", red.Color);
        }

        [Test]
        public void GetNamedWidgets()
        {
            IWidget widget = (IWidget) ObjectFactory.GetNamedInstance(typeof (IWidget), "NotPluggableInstance");
            Assert.IsNotNull(widget);

            ConfigurationWidget confWidget =
                (ConfigurationWidget) ObjectFactory.GetNamedInstance(typeof (IWidget), "BigOne");
            Assert.IsNotNull(confWidget);
        }


        [Test]
        public void GetParentWithDefinedChild()
        {
            Parent parent = ObjectFactory.GetNamedInstance(typeof (Parent), "Jackie") as Parent;
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
            Parent parent = ObjectFactory.GetNamedInstance(typeof (Parent), "Jerry") as Parent;
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
            ObjectFactory.SetDefaultInstanceName(typeof (GrandChild), "Trevor");

            Child child = ObjectFactory.GetNamedInstance(typeof (Child), "Jessica") as Child;


            Assert.IsNotNull(child);
            Assert.AreEqual("Jessica", child.Name);


            GrandChild grandChild = child.MyGrandChild;
            Assert.IsNotNull(grandChild);
            Assert.AreEqual(1979, grandChild.BirthYear, "Has correct BirthYear");
            Assert.IsTrue(grandChild is LeftieGrandChild, "Is a Leftie?");
        }

        [Test]
        public void GetRules()
        {
            ColorRule red = (ColorRule) ObjectFactory.GetNamedInstance(typeof (Rule), "Red");
            Assert.IsNotNull(red);
            Assert.AreEqual("Red", red.Color);

            ColorRule blue = (ColorRule) ObjectFactory.GetNamedInstance(typeof (Rule), "Blue");
            Assert.IsNotNull(blue);
            Assert.AreEqual("Blue", blue.Color);

            GreaterThanRule bigger = (GreaterThanRule) ObjectFactory.GetNamedInstance(typeof (Rule), "Bigger");
            Assert.IsNotNull(bigger);
        }

        [Test]
        public void SingletonInterceptorAlwaysReturnsSameInstance()
        {
            Rule rule1 = (Rule) ObjectFactory.GetNamedInstance(typeof (Rule), "Red");
            Rule rule2 = (Rule) ObjectFactory.GetNamedInstance(typeof (Rule), "Red");
            Rule rule3 = (Rule) ObjectFactory.GetNamedInstance(typeof (Rule), "Red");

            Assert.AreSame(rule1, rule2);
            Assert.AreSame(rule1, rule3);
        }
    }


    public class FilledTarget
    {
        private readonly IGateway _gateway;
        private readonly Rule _rule;

        public FilledTarget(IGateway gateway, Rule rule)
        {
            _gateway = gateway;
            _rule = rule;
        }

        public IGateway Gateway
        {
            get { return _gateway; }
        }

        public Rule Rule
        {
            get { return _rule; }
        }
    }
}