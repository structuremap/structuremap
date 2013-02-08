using System;
using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Testing.Widget;
using System.Linq;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class EnumerableShouldGetAllValuesTester
    {
        public interface IWidget
        {
            void DoSomething();
        }

        public class ColorWidget : IWidget
        {
            private readonly string _Color;

            public ColorWidget(string color)
            {
                _Color = color;
            }

            public string Color { get { return _Color; } }
            public void DoSomething()
            {
                throw new NotImplementedException();
            }
        }


        [Test]
        public void ienumerable_arg_should_get_all_registered()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().AddInstances(o =>
                {
                    o.Type<ColorWidget>().Ctor<string>("color").Is("red");
                    o.Type<ColorWidget>().Ctor<string>("color").Is("blue");
                    o.Type<ColorWidget>().Ctor<string>("color").Is("green");
                });
            });

            container.GetInstance<ClassWithEnumerable>().Widgets.Count().ShouldEqual(3);
        }

        public class ClassWithEnumerable
        {
            private readonly IEnumerable<IWidget> _widgets;

            public ClassWithEnumerable(IEnumerable<IWidget> widgets)
            {
                _widgets = widgets;
            }

            public IEnumerable<IWidget> Widgets { get { return _widgets; } }
        }
    }
}