namespace StructureMap
{
    public enum InstanceScope
    {
        PerRequest,
        Singleton,
        ThreadLocal,
        HttpContext,
        Hybrid,
        HttpSession,
        HybridHttpSession,
        Unique,
        Transient
    }
}