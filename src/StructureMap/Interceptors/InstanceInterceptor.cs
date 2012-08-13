namespace StructureMap.Interceptors
{
    /// <summary>
    /// An InstanceInterceptor can be registered on a per-Instance basis
    /// to act on, or even replace, the object that is created before
    /// it is passed back to the caller.  This is primarily a hook
    /// for runtime AOP scenarios.
    /// </summary>
    public interface InstanceInterceptor
    {
        object Process(object target, IContext context);
    }
}