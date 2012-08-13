using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class DefaultInstanceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        public interface IDefault
        {
        }

        public class DefaultClass : IDefault
        {
        }

        public class ClassWithWidgets
        {
            private readonly List<IWidget> _widgets;

            public ClassWithWidgets(List<IWidget> widgets)
            {
                _widgets = widgets;
            }

            public List<IWidget> Widgets { get { return _widgets; } }
        }

        [Test]
        public void Build_happy_path()
        {
            var mocks = new MockRepository();
            var buildSession =
                mocks.StrictMock<BuildSession>();

            var theDefault = new DefaultClass();


            using (mocks.Record())
            {
                Expect.Call(buildSession.CreateInstance(typeof (IDefault))).Return(theDefault);
            }

            using (mocks.Playback())
            {
                var instance = new DefaultInstance();
                Assert.AreSame(theDefault, instance.Build(typeof (IDefault), buildSession));
            }
        }

        [Test]
        public void Get_description()
        {
            TestUtility.AssertDescriptionIs(new DefaultInstance(), "Default");
        }

        [Test]
        public void use_all_instances_of_an_enumerable_element_type()
        {
            var widget1 = new AWidget();
            var widget2 = new AWidget();
            var widget3 = new AWidget();

            var container = new Container(x =>
            {
                x.For<IWidget>().AddInstances(o =>
                {
                    o.Object(widget1);
                    o.Object(widget2);
                    o.Object(widget3);
                });
            });

            container.GetInstance<ClassWithWidgets>().Widgets.ShouldHaveTheSameElementsAs(widget1, widget2, widget3);
        }
    }
}