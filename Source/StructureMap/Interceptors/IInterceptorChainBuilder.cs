using StructureMap.Attributes;
using StructureMap.Graph;

namespace StructureMap.Interceptors
{
	public interface IInterceptorChainBuilder
	{
		InterceptionChain Build(InstanceScope scope);
	}
}