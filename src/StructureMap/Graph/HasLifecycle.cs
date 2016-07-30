using StructureMap.Pipeline;

namespace StructureMap.Graph
{
    public abstract class HasLifecycle
    {
        private ILifecycle _lifecycle;

        protected void copyLifecycle(HasLifecycle other)
        {
            _lifecycle = other._lifecycle;
        }

        public void SetLifecycleTo<T>() where T : ILifecycle, new()
        {
            _lifecycle = Lifecycles.Get<T>();
        }

        public void SetLifecycleTo(ILifecycle lifecycle)
        {
            _lifecycle = lifecycle;
        }

        public ILifecycle Lifecycle
        {
            get { return _lifecycle; }
        }
    }
}