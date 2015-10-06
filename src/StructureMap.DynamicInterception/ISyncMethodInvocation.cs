namespace StructureMap.DynamicInterception
{
    public interface ISyncMethodInvocation : IMethodInvocation
    {
        IMethodInvocationResult InvokeNext();
    }
}