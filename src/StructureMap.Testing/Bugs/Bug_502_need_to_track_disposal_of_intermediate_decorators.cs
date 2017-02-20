using System;
using Shouldly;
using StructureMap.Testing.Acceptance;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_502_need_to_track_disposal_of_intermediate_decorators
    {
        //[Fact] -- not ready yet.
        public void dispose_both_when_nested_container_is_closed()
        {
            var container = new Container(_ =>
            {
                _.For<IWidget>().Use<DisposableWidget>();
                _.For<IWidget>().DecorateAllWith<WidgetDecorator1>();
                _.For<IWidget>().DecorateAllWith<WidgetDecorator2>();
            });

            IWidget widget;

            using (var nested = container.GetNestedContainer())
            {
                widget = nested.GetInstance<IWidget>();
            }

            var outer = widget.ShouldBeOfType<WidgetDecorator2>();
            outer.WasDisposed.ShouldBeTrue();

            var inner = outer.Inner.ShouldBeOfType<WidgetDecorator1>();
            inner.WasDisposed.ShouldBeTrue();

            inner.Inner.ShouldBeOfType<DisposableWidget>()
                .WasDisposed.ShouldBeTrue();

        }

        public class DisposableWidget : IWidget, IDisposable
        {
            public void Dispose()
            {
                WasDisposed = true;
            }

            public bool WasDisposed { get; set; }
        }
    }

    public class WidgetDecorator1 : IWidget, IDisposable
    {
        public IWidget Inner { get; }

        public WidgetDecorator1(IWidget inner)
        {
            Inner = inner;
        }

        public void Dispose()
        {
            WasDisposed = true;
        }

        public bool WasDisposed { get; set; }
    }

    public class WidgetDecorator2 : WidgetDecorator1
    {
        public WidgetDecorator2(IWidget inner) : base(inner)
        {
        }
    }
}