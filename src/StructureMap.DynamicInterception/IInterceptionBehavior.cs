namespace StructureMap.DynamicInterception
{
    public interface IInterceptionBehavior
    {
        IMethodInvocationResult Intercept(IMethodInvocation methodInvocation);
    }
}