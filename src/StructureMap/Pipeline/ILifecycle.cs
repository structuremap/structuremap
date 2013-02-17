namespace StructureMap.Pipeline
{
    public interface ILifecycle
    {
        string Scope { get; }
        void EjectAll(ILifecycleContext context);
        IObjectCache FindCache(ILifecycleContext context);
    }

    public class TransientLifecycle : ILifecycle
    {
        public string Scope
        {
            get { return InstanceScope.Transient.ToString(); }
        }

        public void EjectAll(ILifecycleContext context)
        {
            FindCache(context).DisposeAndClear();
        }

        public IObjectCache FindCache(ILifecycleContext context)
        {
            return context.Transients;
        }
    }
}