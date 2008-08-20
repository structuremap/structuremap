using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class BuildStackTester
    {
        [Test]
        public void Create_build_stack_and_the_root_is_from_the_ctor()
        {
            var root = new BuildFrame(typeof(IWidget), "Blue", typeof(ColorWidget));
            BuildStack stack = new BuildStack();
            stack.Push(root);

            stack.Root.ShouldBeTheSameAs(root);
        }

        [Test]
        public void push_a_new_BuildFrame_onto_the_stack()
        {
            var root = new BuildFrame(typeof(IWidget), "Root", typeof(ColorWidget));
            var frame1 = new BuildFrame(typeof(IWidget), "Frame1", typeof(ColorWidget));
            var frame2 = new BuildFrame(typeof(IWidget), "Frame2", typeof(ColorWidget));
            BuildStack stack = new BuildStack();
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
}