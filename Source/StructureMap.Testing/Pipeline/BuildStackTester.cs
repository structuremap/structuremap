using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class BuildStackTester
    {
        [Test]
        public void Create_build_stack_and_the_root_is_from_the_ctor()
        {
            var root = new BuildFrame(typeof (IWidget), "Blue", typeof (ColorWidget));
            var stack = new BuildStack();
            stack.Push(root);

            stack.Root.ShouldBeTheSameAs(root);
        }

        [Test]
        public void push_a_new_BuildFrame_onto_the_stack()
        {
            var root = new BuildFrame(typeof (IWidget), "Root", typeof (ColorWidget));
            var frame1 = new BuildFrame(typeof (IWidget), "Frame1", typeof (ColorWidget));
            var frame2 = new BuildFrame(typeof (IWidget), "Frame2", typeof (ColorWidget));
            var stack = new BuildStack();
            stack.Push(root);

            stack.Push(frame1);
            stack.Current.ShouldBeTheSameAs(frame1);
            stack.Parent.ShouldBeTheSameAs(root);
            stack.Root.ShouldBeTheSameAs(root);

            stack.Push(frame2);
            stack.Parent.ShouldBeTheSameAs(frame1);
            stack.Current.ShouldBeTheSameAs(frame2);
            stack.Root.ShouldBeTheSameAs(root);

            stack.Pop();
            stack.Parent.ShouldBeTheSameAs(root);
            stack.Current.ShouldBeTheSameAs(frame1);
            stack.Pop();
            stack.Current.ShouldBeTheSameAs(root);
        }
    }

    [TestFixture]
    public class when_using_build_frame_contains
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void true_if_matching()
        {
            var frame1 = new BuildFrame(typeof (IWidget), "red", typeof (ColorWidget));
            var frame2 = new BuildFrame(typeof (IWidget), "red", typeof (ColorWidget));
            var frame3 = new BuildFrame(typeof (IWidget), "green", typeof (ColorWidget));

            frame1.Contains(frame2).ShouldBeTrue();
            frame1.Contains(frame3).ShouldBeFalse();

            frame3.Attach(frame2);

            frame3.Contains(frame1).ShouldBeTrue();
        }
    }
}