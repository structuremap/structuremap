using System;
using System.Linq;
using NUnit.Framework;
using StructureMap.Building.Interception;
using StructureMap.Testing.Widget3;
using StructureMap.TypeRules;

namespace StructureMap.Testing.Building.Interception
{
    [TestFixture]
    public class InterceptionPolicyTester
    {
        private ActivatorInterceptor<ITarget> theActivator;
        private InterceptionPolicy<ITarget> thePolicy;

        [SetUp]
        public void SetUp()
        {
            theActivator = new ActivatorInterceptor<ITarget>(x => x.Activate());
            thePolicy = new InterceptionPolicy<ITarget>(theActivator);
        }

        [Test]
        public void description()
        {
            thePolicy.Description.ShouldContain(theActivator.Description);
            thePolicy.Description.ShouldContain("can be cast to " + typeof(ITarget).GetFullName());
        }

        [Test]
        public void determine_interceptors_positive_match()
        {
            thePolicy.DetermineInterceptors(typeof (Target))
                .Single().ShouldBeTheSameAs(theActivator);
        }

        [Test]
        public void determine_interceptions_negative_match()
        {
            thePolicy.DetermineInterceptors(GetType())
                .Any().ShouldBeFalse();
        }

        [Test]
        public void defensive_check_if_interceptor_does_not_fit_the_type()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() => {
                new InterceptionPolicy<IGateway>(theActivator);
            });
        }
    }
}