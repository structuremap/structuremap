namespace StructureMap.Interceptors
{
    public interface InstanceInterceptor
    {
        object Process(object target);
    }
}