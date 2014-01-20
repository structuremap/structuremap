using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Building;

namespace StructureMap.Testing
{
    [TestFixture]
    public class BidirectionalDependencies
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            container = new Container(x => {
                x.For<IBiView>().Use<BiView>();
                x.For<IBiPresenter>().Use<BiPresenter>();

                x.For<IBiGrandparent>().Use<BiGrandparent>();
                x.For<IBiHolder>().Use<BiHolder>();
                x.For<IBiLeaf>().Use<BiLeaf>();
            });
        }

        #endregion

        private Container container;

        [Test]
        public void do_not_blow_up_with_a_stack_overflow_problem()
        {
            // TODO -- make the exceptions better
            var ex = Exception<StructureMapBuildException>.ShouldBeThrownBy(() => {
                container.GetInstance<IBiPresenter>();
            });

            ex.Title.ShouldContain("Bi-directional dependency relationship detected!");
        }

        [Test]
        public void do_not_blow_up_with_a_stack_overflow_problem_2()
        {
            var ex = Exception<StructureMapBuildException>.ShouldBeThrownBy(() =>
            {
                container.GetInstance<IBiHolder>();
            });

            Debug.WriteLine(ex);

            ex.Title.ShouldContain("Bi-directional dependency relationship detected!");
        }
    }

    public interface IBiHolder
    {
        
    }

    public interface IBiGrandparent
    {
        
    }

    public interface IBiLeaf
    {
        
    }

    public class BiHolder : IBiHolder
    {
        public BiHolder(IBiGrandparent grandparent)
        {
        }
    }

    public class BiGrandparent : IBiGrandparent
    {
        public BiGrandparent(IBiLeaf leaf)
        {
        }

    }

    public class BiLeaf : IBiLeaf
    {
        public BiLeaf(IBiHolder holder)
        {
        }
    }


    public interface IBiView
    {
    }

    public interface IBiPresenter
    {
    }

    public class BiView : IBiView
    {
        private readonly IBiPresenter _presenter;

        public BiView(IBiPresenter presenter)
        {
            _presenter = presenter;
        }

        public IBiPresenter Presenter
        {
            get { return _presenter; }
        }
    }

    public class BiPresenter : IBiPresenter
    {
        private readonly IBiView _view;

        public BiPresenter(IBiView view)
        {
            _view = view;
        }

        public IBiView View
        {
            get { return _view; }
        }
    }
}