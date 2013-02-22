namespace StructureMap
{
    public static class InstanceScope
    {
        public const string Singleton = "Singleton";
        public const string ThreadLocal = "ThreadLocal";
        public const string HttpContext = "HttpContext";
        public const string Hybrid = "Hybrid";
        public const string HttpSession = "HttpSession";
        public const string HybridHttpSession = "HybridHttpSession";
        public const string Unique = "Unique";
        public const string Transient = "Transient";
    }
}