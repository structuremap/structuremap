using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using StructureMap.Building.Interception;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Building.Interception
{
    [TestFixture]
    public class InterceptionPoliciesTester
    {
        [Test]
        public void do_not_duplicate_interceptor_policies()
        {
            var theActivator = new ActivatorInterceptor<ITarget>(x => x.Activate());
            var policy1 = new InterceptionPolicy<ITarget>(theActivator);
            var policy2 = new InterceptionPolicy<ITarget>(theActivator);

            policy1.ShouldEqual(policy2);

            var policies = new InterceptorPolicies();
            policies.Add(policy1);
            policies.Add(policy2);
            policies.Add(policy1);
            policies.Add(policy2);

            policies.Policies.Single().ShouldBeTheSameAs(policy1);
        }

        [Test]
        public void select_interceptors()
        {
            var activator1 = new ActivatorInterceptor<ITarget>(x => x.Activate());
            var activator2 = new ActivatorInterceptor<Target>(x => x.UseSession(null));
            var activator3 = new ActivatorInterceptor<Target>(x => x.ThrowUp());
            var activator4 = new ActivatorInterceptor<ITarget>(x => x.Debug());
            var activator5 = new ActivatorInterceptor<IGateway>(x => x.DoSomething());

            var policies = new InterceptorPolicies();
            policies.Add(activator1.ToPolicy());
            policies.Add(activator2.ToPolicy());
            policies.Add(activator3.ToPolicy());
            policies.Add(activator4.ToPolicy());
            policies.Add(activator5.ToPolicy());

            policies.SelectInterceptors(typeof(Target))
                .ShouldHaveTheSameElementsAs(activator1, activator2, activator3, activator4);

            policies.SelectInterceptors(typeof(ITarget))
                .ShouldHaveTheSameElementsAs(activator1, activator4);

            policies.SelectInterceptors(typeof(IGateway))
                .ShouldHaveTheSameElementsAs(activator5);
        }
    }
}