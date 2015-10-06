using System.Threading.Tasks;

namespace StructureMap.DynamicInterception
{
    public interface IAsyncMethodInvocation : IMethodInvocation
    {
        Task<IMethodInvocationResult> InvokeNextAsync();
    }
}