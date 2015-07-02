using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Building;
using StructureMap.Building.Interception;
using StructureMap.Diagnostics;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Building
{
    [TestFixture]
    public class BuildPlan_AcceptVisitor_Behavior
    {
        [Test]
        public void accept_a_visitor_with_no_interception_and_a_visitable_inner()
        {
            var mocks = new MockRepository();

            var visitor = mocks.StrictMock<IBuildPlanVisitor>();
            var inner = mocks.StrictMultiMock<IDependencySource>(typeof (IBuildPlanVisitable));

            var plan = new BuildPlan(typeof (IGateway), new ObjectInstance(new StubbedGateway()), inner, null);

            using (mocks.Ordered())
            {
                visitor.Instance(plan.PluginType, plan.Instance);
                inner.As<IBuildPlanVisitable>().AcceptVisitor(visitor);
            }

            mocks.ReplayAll();

            using (mocks.Playback())
            {
                plan.AcceptVisitor(visitor);
            }
        }

        [Test]
        public void accept_a_visitor_with_no_interception_and_a_simpler_inner_builder()
        {
            var mocks = new MockRepository();

            var visitor = mocks.StrictMock<IBuildPlanVisitor>();
            var inner = mocks.StrictMock<IDependencySource>();

            var plan = new BuildPlan(typeof (IGateway), new ObjectInstance(new StubbedGateway()), inner, null);

            using (mocks.Ordered())
            {
                visitor.Instance(plan.PluginType, plan.Instance);
                visitor.InnerBuilder(inner);
            }

            mocks.ReplayAll();

            using (mocks.Playback())
            {
                plan.AcceptVisitor(visitor);
            }
        }

        [Test]
        public void accept_a_visitor_with_interception_and_a_visitable_inner()
        {
            var mocks = new MockRepository();

            var visitor = mocks.StrictMock<IBuildPlanVisitor>();
            var inner = mocks.StrictMultiMock<IDependencySource>(typeof (IBuildPlanVisitable));
            var interception = mocks.StrictMock<IInterceptionPlan>();

            var plan = new BuildPlan(typeof (IGateway), new ObjectInstance(new StubbedGateway()), inner, interception);

            using (mocks.Ordered())
            {
                visitor.Instance(plan.PluginType, plan.Instance);
                interception.AcceptVisitor(visitor);
                inner.As<IBuildPlanVisitable>().AcceptVisitor(visitor);
            }

            mocks.ReplayAll();

            using (mocks.Playback())
            {
                plan.AcceptVisitor(visitor);
            }
        }


        [Test]
        public void accept_a_visitor_with_interception_and_a_simpler_inner_builder()
        {
            var mocks = new MockRepository();

            var visitor = mocks.StrictMock<IBuildPlanVisitor>();
            var inner = mocks.StrictMock<IDependencySource>();
            var interception = mocks.StrictMock<IInterceptionPlan>();


            var plan = new BuildPlan(typeof (IGateway), new ObjectInstance(new StubbedGateway()), inner, interception);

            using (mocks.Ordered())
            {
                visitor.Instance(plan.PluginType, plan.Instance);
                interception.AcceptVisitor(visitor);
                visitor.InnerBuilder(inner);
            }

            mocks.ReplayAll();

            using (mocks.Playback())
            {
                plan.AcceptVisitor(visitor);
            }
        }
    }
}