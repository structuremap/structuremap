using Shouldly;
using StructureMap.Building;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;
using System.Collections.Generic;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class DefaultInstanceTester
    {
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

        [Fact]
        public void Get_description()
        {
            new DefaultInstance().Description.ShouldBe("Default");
        }

        [Fact]
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

        [Fact]
        public void to_dependency_source_when_not_an_enum()
        {
            new DefaultInstance().ToDependencySource(typeof(IGateway))
                .ShouldBeOfType<DefaultDependencySource>()
                .DependencyType.ShouldBe(typeof(IGateway));
        }

        [Fact]
        public void to_dependency_source_when_an_array()
        {
            var enumerationType = typeof(IGateway[]);
            new DefaultInstance().ToDependencySource(enumerationType)
                .ShouldBe(new AllPossibleValuesDependencySource(enumerationType));
        }

        [Fact]
        public void to_dependency_source_when_an_IEnumerable()
        {
            var enumerationType = typeof(IEnumerable<IGateway>);
            new DefaultInstance().ToDependencySource(enumerationType)
                .ShouldBe(new AllPossibleValuesDependencySource(enumerationType));
        }

        [Fact]
        public void to_dependency_source_when_an_IList()
        {
            var enumerationType = typeof(IList<IGateway>);
            new DefaultInstance().ToDependencySource(enumerationType)
                .ShouldBe(new AllPossibleValuesDependencySource(enumerationType));
        }

        [Fact]
        public void to_dependency_source_when_a_concrete_List()
        {
            var enumerationType = typeof(IList<IGateway>);
            new DefaultInstance().ToDependencySource(enumerationType)
                .ShouldBe(new AllPossibleValuesDependencySource(enumerationType));
        }
    }
}