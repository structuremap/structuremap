using StructureMap.Building.Interception;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;
using StructureMap.TypeRules;
using System;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Building.Interception
{
    public class InterceptorPolicyTester
    {
        private readonly ActivatorInterceptor<ITarget> theActivator;
        private readonly InterceptorPolicy<ITarget> thePolicy;

        public InterceptorPolicyTester()
        {
            theActivator = new ActivatorInterceptor<ITarget>(x => x.Activate());
            thePolicy = new InterceptorPolicy<ITarget>(theActivator);
        }

        [Fact]
        public void description()
        {
            thePolicy.Description.ShouldContain(theActivator.Description);
            thePolicy.Description.ShouldContain("can be cast to " + typeof(ITarget).GetFullName());
        }

        [Fact]
        public void determine_interceptors_positive_match()
        {
            thePolicy.DetermineInterceptors(typeof(Target), new SmartInstance<Target>())
                .Single().ShouldBeTheSameAs(theActivator);
        }

        [Fact]
        public void determine_interceptions_negative_match()
        {
            thePolicy.DetermineInterceptors(typeof(ITarget), new ConstructorInstance(GetType()))
                .Any().ShouldBeFalse();
        }

        [Fact]
        public void matches_exactly_on_plugin_type_if_the_interceptor_is_a_decorator()
        {
            var policy = new InterceptorPolicy<ITarget>(new FuncInterceptor<ITarget>(x => new DecoratedTarget(x)));
            policy.DetermineInterceptors(typeof(Target), new SmartInstance<Target>())
                .Any().ShouldBeFalse();

            policy.DetermineInterceptors(typeof(ITarget), new SmartInstance<Target>())
                .Any().ShouldBeTrue();
        }

        [Fact]
        public void uses_the_filter_on_instance()
        {
            thePolicy.Filter = i => i.Name == "good";

            thePolicy.DetermineInterceptors(typeof(Target), new SmartInstance<Target>().Named("good"))
                .Any().ShouldBeTrue();

            thePolicy.DetermineInterceptors(typeof(Target), new SmartInstance<Target>().Named("bad"))
                .Any().ShouldBeFalse();
        }

        [Fact]
        public void defensive_check_if_interceptor_does_not_fit_the_type()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(
                () => { new InterceptorPolicy<IGateway>(theActivator); });
        }
    }
}