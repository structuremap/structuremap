namespace StructureMap.Pipeline
{
    public interface ILifecycle
    {
        string Description { get; }
        void EjectAll(ILifecycleContext context);
        IObjectCache FindCache(ILifecycleContext context);
    }

    public interface IAppropriateForNestedContainer{}

    public abstract class LifecycleBase : ILifecycle
    {
        public string Description
        {
            get { return GetType().Name.Replace("Lifecycle", string.Empty); }
        }

        public abstract void EjectAll(ILifecycleContext context);
        public abstract IObjectCache FindCache(ILifecycleContext context);
    }

    public class TransientLifecycle : LifecycleBase, IAppropriateForNestedContainer
    {
        public override void EjectAll(ILifecycleContext context)
        {
            FindCache(context).DisposeAndClear();
        }

        public override IObjectCache FindCache(ILifecycleContext context)
        {
            return context.Transients;
        }
    }
}