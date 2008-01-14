namespace StructureMap.Interceptors
{
    public class NulloInterceptor : InstanceInterceptor
    {
        public object Process(object target)
        {
            return target;
        }
    }
}