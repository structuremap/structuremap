using Castle.DynamicProxy;
using StructureMap.Pipeline;
using System.Linq;

namespace StructureMap.AutoFactory
{
    public class FactoryInterceptor : IInterceptor
    {
        private readonly IContainer _container;
        private readonly IAutoFactoryConventionProvider _conventionProvider;

        public FactoryInterceptor(IContainer container, IAutoFactoryConventionProvider conventionProvider)
        {
            _container = container;
            _conventionProvider = conventionProvider;
        }

        public void Intercept(IInvocation invocation)
        {
            var methodDefinition = _conventionProvider.GetMethodDefinition(invocation.Method, invocation.Arguments);

            if (methodDefinition == null)
            {
                return;
            }

            switch (methodDefinition.MethodType)
            {
                case AutoFactoryMethodType.GetInstance:
                    var explicitArguments = methodDefinition.ExplicitArguments ?? new ExplicitArguments();
                    invocation.ReturnValue = !string.IsNullOrEmpty(methodDefinition.InstanceName)
                        ? _container.TryGetInstance(methodDefinition.InstanceType, explicitArguments, methodDefinition.InstanceName)
                        : _container.TryGetInstance(methodDefinition.InstanceType, explicitArguments);
                    break;

                case AutoFactoryMethodType.GetNames:
                    invocation.ReturnValue = _container.Model.AllInstances
                        .Where(x => x.PluginType == methodDefinition.InstanceType)
                        .Select(x => x.Instance.HasExplicitName() ? x.Name : string.Empty)
                        .ToList();
                    break;
            }
        }
    }
}