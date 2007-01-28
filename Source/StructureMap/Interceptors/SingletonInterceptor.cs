using System.Collections;

namespace StructureMap.Interceptors
{
    /// <summary>
    /// The SingletonInterceptor is a GoF Decorator around an IInstanceFactory that ensures that 
    /// only one instance is created for a given InstanceKey as a more testable alternative to 
    /// the GoF Singleton pattern. 
    /// </summary>
    [Pluggable("Singleton")]
    public class SingletonInterceptor : CacheInterceptor
    {
        private IDictionary _instances;

        [DefaultConstructor]
        public SingletonInterceptor() : this(new Hashtable())
        {
        }

        public SingletonInterceptor(IDictionary instances) : base()
        {
            _instances = instances;
        }


        protected override void cache(string instanceKey, object instance)
        {
            _instances.Add(instanceKey, instance);
        }

        protected override bool isCached(string instanceKey)
        {
            return _instances.Contains(instanceKey);
        }

        protected override object getInstance(string instanceKey)
        {
            return _instances[instanceKey];
        }

        public override object Clone()
        {
            return new SingletonInterceptor();
        }
    }
}