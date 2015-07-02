using System;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using StructureMap.Building.Interception;
using StructureMap.Pipeline;
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
            var policy1 = new InterceptorPolicy<ITarget>(theActivator);
            var policy2 = new InterceptorPolicy<ITarget>(theActivator);

            policy1.ShouldBe(policy2);

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


            policies.SelectInterceptors(typeof (ITarget), new SmartInstance<Target>())
                .ShouldHaveTheSameElementsAs(activator1, activator2, activator3, activator4);

            policies.SelectInterceptors(typeof (ITarget), new SmartInstance<ATarget>())
                .ShouldHaveTheSameElementsAs(activator1, activator4);

            policies.SelectInterceptors(typeof (ITarget), new SmartInstance<StubbedGateway>())
                .ShouldHaveTheSameElementsAs(activator5);
        }

        [Test]
        public void apply_policy_selectively_with_a_func()
        {
            var activator1 = new ActivatorInterceptor<ITarget>(x => x.Activate());
            var policy = new InterceptorPolicy<ITarget>(activator1, i => i.Name.StartsWith("A"));

            var container = new Container(x =>
            {
                x.Policies.Interceptors(policy);
                x.For<ITarget>().AddInstances(targets =>
                {
                    targets.Type<ATarget>().Named("A");
                    targets.Type<ATarget>().Named("A1");
                    targets.Type<ATarget>().Named("A2");
                    targets.Type<ATarget>().Named("B");
                    targets.Type<ATarget>().Named("C");
                    targets.Type<ATarget>().Named("D");
                });
            });

            container.GetInstance<ITarget>("A").ShouldBeOfType<ATarget>().WasActivated.ShouldBeTrue();
            container.GetInstance<ITarget>("A1").ShouldBeOfType<ATarget>().WasActivated.ShouldBeTrue();
            container.GetInstance<ITarget>("A2").ShouldBeOfType<ATarget>().WasActivated.ShouldBeTrue();
            container.GetInstance<ITarget>("B").ShouldBeOfType<ATarget>().WasActivated.ShouldBeFalse();
            container.GetInstance<ITarget>("C").ShouldBeOfType<ATarget>().WasActivated.ShouldBeFalse();
            container.GetInstance<ITarget>("D").ShouldBeOfType<ATarget>().WasActivated.ShouldBeFalse();
        }
    }

    public class ATarget : ITarget
    {
        public void Activate()
        {
            WasActivated = true;
        }

        public bool WasActivated { get; set; }

        public void Debug()
        {
            throw new NotImplementedException();
        }
    }

    public class BTarget : ATarget
    {
    }

    public class CTarget : ATarget
    {
    }

    public class DTarget : ATarget
    {
    }
}