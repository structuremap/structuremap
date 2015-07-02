using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Building;
using StructureMap.Diagnostics;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Building
{
    [TestFixture]
    public class Dependency_Visiting_Tester
    {
        private IDependencyVisitor theVisitor;

        [SetUp]
        public void SetUp()
        {
            theVisitor = MockRepository.GenerateMock<IDependencyVisitor>();
        }


        [Test]
        public void Constant_accepts_Visitor()
        {
            var constant = new Constant(typeof (string), "a");
            constant.AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.Constant(constant));
        }

        [Test]
        public void LambdaInstance_accepts_Visitor()
        {
            var instance = new LambdaInstance<IGateway>(() => null);
            instance.As<IDependencySource>().AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.Dependency(instance));
        }

        [Test]
        public void DependencyProblem_accepts_Visitor()
        {
            var problem = new DependencyProblem();

            problem.AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.Problem(problem));
        }

        [Test]
        public void ReferencedDependencySource_accepts_Visitor()
        {
            var source = new ReferencedDependencySource(typeof (IGateway), "Green");

            source.AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.Referenced(source));
        }

        [Test]
        public void LifecycleDependencySource_accepts_Visitor()
        {
            var source = new LifecycleDependencySource(typeof (IGateway), new SmartInstance<StubbedGateway>());
            source.AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.Lifecycled(source));
        }

        [Test]
        public void DefaultDependencySource_accepts_Visitor()
        {
            var source = new DefaultDependencySource(typeof (IGateway));

            source.AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.Default(source.ReturnedType));
        }

        [Test]
        public void ArrayDependencySource_accepts_Visitor()
        {
            var source = new ArrayDependencySource(typeof (IGateway));

            source.AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.InlineEnumerable(source));
        }

        [Test]
        public void ConcreteBuild_accepts_Visitor()
        {
            var build = new ConcreteBuild<StubbedGateway>();
            build.AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.Concrete(build));
        }

        [Test]
        public void AllPossibleValuesDependencySource_accepts_Visitor()
        {
            var source = new AllPossibleValuesDependencySource(typeof (IGateway[]));
            source.AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.AllPossibleOf(typeof (IGateway)));
        }
    }
}