using System.Threading.Tasks;

namespace StructureMap.DynamicInterception
{
    public interface IAsyncInterceptionBehavior : IInterceptionBehavior
    {
        Task<IMethodInvocationResult> InterceptAsync(IAsyncMethodInvocation methodInvocation);
    }
}