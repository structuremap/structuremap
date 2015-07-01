using System;
using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Building.Interception;
using StructureMap.Pipeline;
using StructureMap.Testing.Building.Interception;

namespace StructureMap.Testing.Building
{
    [TestFixture]
    public class BuildPlanTester
    {
        private Instance theInstance;
        private BuildTarget theTarget;
        private IInterceptor[] theInterceptors;
        private IDependencySource theInner;
        private Lazy<BuildPlan> _plan;
        private FakeBuildSession theSession;

        [SetUp]
        public void SetUp()
        {
            theTarget = new BuildTarget();
            theInstance = new ObjectInstance(theTarget);

            theInterceptors = new IInterceptor[0];

            theInner = Constant.For(theTarget);

            _plan =
                new Lazy<BuildPlan>(() => new BuildPlan(typeof (IBuildTarget), theInstance, theInner, new Policies(), theInterceptors));

            theSession = new FakeBuildSession();
        }

        private BuildPlan thePlan
        {
            get
            {
                return _plan.Value;
            }
        }

        [Test]
        public void create_happy_path_with_no_interceptors()
        {
            thePlan.Build(theSession, theSession).ShouldBeTheSameAs(theTarget);
        }

        [Test]
        public void create_happy_path_with_a_single_decorate_interceptor()
        {
            theInterceptors = new IInterceptor[]
            {
                new FuncInterceptor<IBuildTarget>(x => new TargetDecorator(x))
            };

            thePlan.Build(theSession, theSession)
                .ShouldBeOfType<TargetDecorator>()
                .Inner.ShouldBeTheSameAs(theTarget);
        }

        [Test]
        public void the_construction_blows_up_get_the_description_of_the_instance()
        {
            theInner = new ConcreteBuild<ThrowsUpTarget>();

            var ex = Exception<StructureMapBuildException>.ShouldBeThrownBy(() => {
                thePlan.Build(theSession, theSession);
            });

            ex.Message.ShouldContain(theInstance.Description);
            ex.Message.ShouldContain(thePlan.Description);
            ex.Message.ShouldContain("new ThrowsUpTarget()");
        }

        [Test]
        public void description_when_the_instance_has_a_name()
        {
            theInstance.Name = "Red";

            thePlan.Description.ShouldBe("Instance of StructureMap.Testing.Building.IBuildTarget ('Red')");
        }

        [Test]
        public void description_when_the_instance_type_does_not_match_the_concrete_type()
        {
            thePlan.Description.ShouldBe("Instance of StructureMap.Testing.Building.IBuildTarget (StructureMap.Testing.Building.BuildTarget)");
        }

        [Test]
        public void description_when_the_instance_concrete_type_is_indeterminate()
        {
            theInstance = new RiggedInstance(null);
            thePlan.Description.ShouldBe("Instance of StructureMap.Testing.Building.IBuildTarget");
        }

        [Test]
        public void description_when_the_instance_concrete_type_is_the_plugin_type()
        {
            theInstance = new RiggedInstance(typeof(IBuildTarget));
            thePlan.Description.ShouldBe("Instance of StructureMap.Testing.Building.IBuildTarget");
        }
    }

    public class RiggedInstance : Instance
    {
        private readonly Type _returnedType;

        public RiggedInstance(Type returnedType)
        {
            _returnedType = returnedType;
        }

        public override string Description
        {
            get { return "Nothing!"; }
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            throw new NotImplementedException();
        }

        public override Type ReturnedType
        {
            get { return _returnedType; }
        }
    }

    public interface IBuildTarget
    {
        
    }

    public class ThrowsUpTarget : IBuildTarget
    {
        public ThrowsUpTarget()
        {
            throw new DivideByZeroException("You shall not pass!");
        }
    }

    public class BuildTarget : IBuildTarget
    {
        
    }

    public class TargetDecorator : IBuildTarget
    {
        private readonly IBuildTarget _inner;

        public TargetDecorator(IBuildTarget inner)
        {
            _inner = inner;
        }

        public IBuildTarget Inner
        {
            get { return _inner; }
        }
    }
}