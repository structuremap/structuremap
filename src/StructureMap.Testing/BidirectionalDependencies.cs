using System.Diagnostics;
using NUnit.Framework;

namespace StructureMap.Testing
{
    [TestFixture]
    public class BidirectionalDependencies
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            container = new Container(x =>
            {
                x.For<IBiView>().Use<BiView>();
                x.For<IBiPresenter>().Use<BiPresenter>();
            });
        }

        #endregion

        private Container container;

        [Test]
        public void do_not_blow_up_with_a_stack_overflow_problem()
        {
            try
            {
                container.GetInstance<BiView>();
                Assert.Fail("Should have thrown error");
            }
            catch (StructureMapException e)
            {
                Debug.WriteLine(e.ToString());
                e.ErrorCode.ShouldEqual(295);
            }
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

        public IBiPresenter Presenter { get { return _presenter; } }
    }

    public class BiPresenter : IBiPresenter
    {
        private readonly IBiView _view;

        public BiPresenter(IBiView view)
        {
            _view = view;
        }

        public IBiView View { get { return _view; } }
    }
}