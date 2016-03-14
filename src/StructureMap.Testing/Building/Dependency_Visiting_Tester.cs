using Rhino.Mocks;
using StructureMap.Building;
using StructureMap.Diagnostics;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;
using Xunit;

namespace StructureMap.Testing.Building
{
    public class Dependency_Visiting_Tester
    {
        private readonly IDependencyVisitor theVisitor;

        public Dependency_Visiting_Tester()
        {
            theVisitor = MockRepository.GenerateMock<IDependencyVisitor>();
        }

        [Fact]
        public void Constant_accepts_Visitor()
        {
            var constant = new Constant(typeof(string), "a");
            constant.AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.Constant(constant));
        }

        [Fact]
        public void LambdaInstance_accepts_Visitor()
        {
            var instance = new LambdaInstance<IGateway>(() => null);
            instance.As<IDependencySource>().AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.Dependency(instance));
        }

        [Fact]
        public void DependencyProblem_accepts_Visitor()
        {
            var problem = new DependencyProblem();

            problem.AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.Problem(problem));
        }

        [Fact]
        public void ReferencedDependencySource_accepts_Visitor()
        {
            var source = new ReferencedDependencySource(typeof(IGateway), "Green");

            source.AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.Referenced(source));
        }

        [Fact]
        public void LifecycleDependencySource_accepts_Visitor()
        {
            var source = new LifecycleDependencySource(typeof(IGateway), new SmartInstance<StubbedGateway>());
            source.AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.Lifecycled(source));
        }

        [Fact]
        public void DefaultDependencySource_accepts_Visitor()
        {
            var source = new DefaultDependencySource(typeof(IGateway));

            source.AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.Default(source.ReturnedType));
        }

        [Fact]
        public void ArrayDependencySource_accepts_Visitor()
        {
            var source = new ArrayDependencySource(typeof(IGateway));

            source.AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.InlineEnumerable(source));
        }

        [Fact]
        public void ConcreteBuild_accepts_Visitor()
        {
            var build = new ConcreteBuild<StubbedGateway>();
            build.AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.Concrete(build));
        }

        [Fact]
        public void AllPossibleValuesDependencySource_accepts_Visitor()
        {
            var source = new AllPossibleValuesDependencySource(typeof(IGateway[]));
            source.AcceptVisitor(theVisitor);

            theVisitor.AssertWasCalled(x => x.AllPossibleOf(typeof(IGateway)));
        }
    }
}