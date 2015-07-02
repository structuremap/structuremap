using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;
using StructureMap.Building;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

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

            public List<IWidget> Widgets
            {
                get { return _widgets; }
            }
        }


        [Test]
        public void Get_description()
        {
            new DefaultInstance().Description.ShouldBe("Default");
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

        [Test]
        public void to_dependency_source_when_not_an_enum()
        {
            new DefaultInstance().ToDependencySource(typeof (IGateway))
                .ShouldBeOfType<DefaultDependencySource>()
                .DependencyType.ShouldBe(typeof (IGateway));
        }

        [Test]
        public void to_dependency_source_when_an_array()
        {
            var enumerationType = typeof (IGateway[]);
            new DefaultInstance().ToDependencySource(enumerationType)
                .ShouldBe(new AllPossibleValuesDependencySource(enumerationType));
        }

        [Test]
        public void to_dependency_source_when_an_IEnumerable()
        {
            var enumerationType = typeof (IEnumerable<IGateway>);
            new DefaultInstance().ToDependencySource(enumerationType)
                .ShouldBe(new AllPossibleValuesDependencySource(enumerationType));
        }


        [Test]
        public void to_dependency_source_when_an_IList()
        {
            var enumerationType = typeof (IList<IGateway>);
            new DefaultInstance().ToDependencySource(enumerationType)
                .ShouldBe(new AllPossibleValuesDependencySource(enumerationType));
        }

        [Test]
        public void to_dependency_source_when_a_concrete_List()
        {
            var enumerationType = typeof (IList<IGateway>);
            new DefaultInstance().ToDependencySource(enumerationType)
                .ShouldBe(new AllPossibleValuesDependencySource(enumerationType));
        }
    }
}