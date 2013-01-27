using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.TestData;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;
using IList=System.Collections.IList;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class FullStackFacadeTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            DataMother.WriteDocument("SampleConfig.xml");
            DataMother.WriteDocument("FullTesting.XML");

            ObjectFactory.Initialize(x => { });
        }

        #endregion

        public class AClass
        {
            private readonly string _name;

            public AClass(string name)
            {
                _name = name;
            }

            public string Name { get { return _name; } }
        }

        [Test]
        public void BuildAllInstances()
        {
            DataMother.WriteDocument("FullTesting.XML");

            PluginGraph pluginGraph = DataMother.GetDiagnosticPluginGraph("SampleConfig.xml");

            var manager = new Container(pluginGraph);

            IList list = manager.GetAllInstances(typeof (IWidget));

            Assert.AreEqual(9, list.Count);

            foreach (object target in list)
            {
                Assert.IsTrue(target is IWidget);
            }
        }

        [Test]
        public void ChangeDefault()
        {
            ObjectFactory.Configure(x => { x.For<IWidget>().Use("Green"); });

            var green = (ColorWidget) ObjectFactory.GetInstance(typeof (IWidget));
            Assert.IsNotNull(green);
            Assert.AreEqual("Green", green.Color);
        }

        [Test]
        public void ChangeDefaultWithGenericMethod()
        {
            ObjectFactory.Configure(x => { x.For<IWidget>().Use("Green"); });

            var green = (ColorWidget) ObjectFactory.GetInstance<IWidget>();
            Assert.IsNotNull(green);
            Assert.AreEqual("Green", green.Color);
        }

        [Test]
        public void FillDependenc1ies2()
        {
            var target = ObjectFactory.GetInstance<FilledTarget>();
            Assert.IsNotNull(target.Gateway);
            Assert.IsNotNull(target.Rule);
        }

        [Test]
        public void FillDependencies()
        {
            var target = (FilledTarget) ObjectFactory.GetInstance(typeof (FilledTarget));
            Assert.IsNotNull(target.Gateway);
            Assert.IsNotNull(target.Rule);
        }

        [Test]
        public void GetChildWithDefinedGrandChild()
        {
            var child = ObjectFactory.GetNamedInstance(typeof (Child), "Tom") as Child;
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
            var child = ObjectFactory.GetNamedInstance(typeof (Child), "Monte") as Child;
        }

        [Test]
        public void GetChildWithReferencedGrandChild()
        {
            var child = ObjectFactory.GetNamedInstance(typeof (Child), "Marsha") as Child;
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
            var red = (ColorWidget) ObjectFactory.GetInstance(typeof (IWidget));
            Assert.IsNotNull(red);
            Assert.AreEqual("Red", red.Color);
        }

        [Test]
        public void GetNamedWidgets()
        {
            var widget = (IWidget) ObjectFactory.GetNamedInstance(typeof (IWidget), "NotPluggableInstance");
            Assert.IsNotNull(widget);

            var confWidget =
                (ConfigurationWidget) ObjectFactory.GetNamedInstance(typeof (IWidget), "BigOne");
            Assert.IsNotNull(confWidget);
        }


        [Test]
        public void GetParentWithDefinedChild()
        {
            var parent = ObjectFactory.GetNamedInstance(typeof (Parent), "Jackie") as Parent;
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
            var parent = ObjectFactory.GetNamedInstance(typeof (Parent), "Jerry") as Parent;
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
            ObjectFactory.Configure(x => { x.For<GrandChild>().Use("Trevor"); });

            var child = ObjectFactory.GetNamedInstance(typeof (Child), "Jessica") as Child;


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
            var red = (ColorRule) ObjectFactory.GetNamedInstance(typeof (Rule), "Red");
            Assert.IsNotNull(red);
            Assert.AreEqual("Red", red.Color);

            var blue = (ColorRule) ObjectFactory.GetNamedInstance(typeof (Rule), "Blue");
            Assert.IsNotNull(blue);
            Assert.AreEqual("Blue", blue.Color);

            var bigger = (GreaterThanRule) ObjectFactory.GetNamedInstance(typeof (Rule), "Bigger");
            Assert.IsNotNull(bigger);
        }

        [Test]
        public void If_there_is_only_one_instance_of_a_type_use_that_as_default()
        {
            var target = new AClass("Me");

            var container = new Container(registry => registry.For<AClass>().Use(target));

            Assert.AreSame(target, container.GetInstance<AClass>());
        }

        [Test]
        public void SingletonInterceptorAlwaysReturnsSameInstance()
        {
            var rule1 = (Rule) ObjectFactory.GetNamedInstance(typeof (Rule), "Red");
            var rule2 = (Rule) ObjectFactory.GetNamedInstance(typeof (Rule), "Red");
            var rule3 = (Rule) ObjectFactory.GetNamedInstance(typeof (Rule), "Red");

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

        public IGateway Gateway { get { return _gateway; } }

        public Rule Rule { get { return _rule; } }
    }
}