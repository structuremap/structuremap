using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
using NUnit.Framework;
using StructureMap.Testing;

namespace FubuMVC.StructureMap3.Testing.Compliance
{
    [TestFixture]
    public class ServiceArgument_Injection_Compliance
    {
        [Test]
        public void can_inject_arguments_into_the_behavior_factory()
        {
            var standInCurrentHttpRequest = new StandInCurrentHttpRequest();
            var inMemoryFubuRequest = new InMemoryFubuRequest();

            var arguments = new ServiceArguments()
                .With<ICurrentHttpRequest>(standInCurrentHttpRequest)
                .With<IFubuRequest>(inMemoryFubuRequest);

            var behavior = ContainerFacilitySource
                .BuildBehavior(arguments, ObjectDef.ForType<GuyWhoNeedsRequest>(), x => { })
                .As<GuyWhoNeedsRequest>();

            behavior.Http.ShouldBeTheSameAs(standInCurrentHttpRequest);
            behavior.Request.ShouldBeTheSameAs(inMemoryFubuRequest);
        }
    }

    public class GuyWhoNeedsRequest : IActionBehavior
    {
        private readonly ICurrentHttpRequest _http;
        private readonly IFubuRequest _request;

        public GuyWhoNeedsRequest(ICurrentHttpRequest http, IFubuRequest request)
        {
            _http = http;
            _request = request;
        }

        public ICurrentHttpRequest Http
        {
            get { return _http; }
        }

        public IFubuRequest Request
        {
            get { return _request; }
        }

        public void Invoke()
        {
            throw new System.NotImplementedException();
        }

        public void InvokePartial()
        {
            throw new System.NotImplementedException();
        }
    }
}