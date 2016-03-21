using StructureMap.Testing.Acceptance;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace StructureMap.Testing
{
    public class Resolving_ienumerable_types
    {
        private Container container;

        public Resolving_ienumerable_types()
        {
            container = new Container(x =>
            {
                x.For<IWidget>().Add<AWidget>();
                x.For<IWidget>().Add<BWidget>();
                x.For<IWidget>().Add<CWidget>();
            });
        }

        [Fact]
        public void if_no_explicit_registration_by_list_return_all()
        {
            container.GetInstance<IList<IWidget>>()
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(AWidget), typeof(BWidget), typeof(CWidget));
        }

        [Fact]
        public void if_no_explicit_registration_by_enumerable_return_all()
        {
            container.GetInstance<IEnumerable<IWidget>>()
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(AWidget), typeof(BWidget), typeof(CWidget));
        }

        [Fact]
        public void if_no_explicit_registration_by_array_return_all()
        {
            container.GetInstance<IWidget[]>()
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(AWidget), typeof(BWidget), typeof(CWidget));
        }

        [Fact]
        public void can_override_array()
        {
            var array = new IWidget[0];
            container.Inject(array);

            container.GetInstance<IWidget[]>()
                .ShouldBeTheSameAs(array);
        }

        [Fact]
        public void can_override_list()
        {
            var list = new List<IWidget>();

            container.Inject<IList<IWidget>>(list);

            container.GetInstance<IList<IWidget>>()
                .ShouldBeTheSameAs(list);
        }

        [Fact]
        public void can_override_enumerable()
        {
            var list = new List<IWidget>();

            container.Inject<IEnumerable<IWidget>>(list);

            container.GetInstance<IEnumerable<IWidget>>()
                .ShouldBeTheSameAs(list);
        }

        [Fact]
        public void can_resolve_a_lazy_for_list()
        {
            container.GetInstance<GuyWithLazyList>()
                .Widgets.Value
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(AWidget), typeof(BWidget), typeof(CWidget));
        }

        [Fact]
        public void can_resolve_a_func_for_list()
        {
            container.GetInstance<GuyWithFuncList>()
                .Widgets()
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof(AWidget), typeof(BWidget), typeof(CWidget));
        }
    }

    public class GuyWithLazyList
    {
        private readonly Lazy<IList<IWidget>> _widgets;

        public GuyWithLazyList(Lazy<IList<IWidget>> widgets)
        {
            _widgets = widgets;
        }

        public Lazy<IList<IWidget>> Widgets
        {
            get { return _widgets; }
        }
    }

    public class GuyWithFuncList
    {
        private readonly Func<IList<IWidget>> _widgets;

        public GuyWithFuncList(Func<IList<IWidget>> widgets)
        {
            _widgets = widgets;
        }

        public Func<IList<IWidget>> Widgets
        {
            get { return _widgets; }
        }
    }
}