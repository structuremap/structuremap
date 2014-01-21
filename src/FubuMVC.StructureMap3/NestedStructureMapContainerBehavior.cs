using System;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using StructureMap;
using StructureMap.Pipeline;

namespace FubuMVC.StructureMap3
{
    public class NestedStructureMapContainerBehavior : IActionBehavior, IDisposable
    {
        private readonly ServiceArguments _arguments;
        private readonly Guid _behaviorId;
        private readonly IContainer _container;
        private IContainer _nested;

        public NestedStructureMapContainerBehavior(IContainer container, ServiceArguments arguments, Guid behaviorId)
        {
            _container = container;
            _arguments = arguments;
            _behaviorId = behaviorId;
        }

        public void Invoke()
        {
            var behavior = StartInnerBehavior();

            behavior.Invoke();
        }

        public IActionBehavior StartInnerBehavior()
        {
            _nested = _container.GetNestedContainer();
            _nested.Configure(x => _arguments.EachService((type, value) => x.For(type).Use(value)));
            var behavior = _nested.GetInstance<IActionBehavior>(_behaviorId.ToString());
            return behavior;
        }

        public void InvokePartial()
        {
            // This should never be called
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            _nested.Dispose();
        }
    }


    public static class ServiceArgumentsExtensions
    {
        public static ExplicitArguments ToExplicitArgs(this ServiceArguments arguments)
        {
            var explicits = new ExplicitArguments();
            arguments.EachService((type, o) => explicits.Set(type, o));

            return explicits;
        }
    }
}