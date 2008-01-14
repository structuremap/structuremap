using System;

namespace StructureMap.Interceptors
{
    public interface TypeInterceptor : InstanceInterceptor
    {
        bool MatchesType(Type type);
    }
}