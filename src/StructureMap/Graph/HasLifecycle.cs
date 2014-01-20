using System;
using StructureMap.Pipeline;

namespace StructureMap.Graph
{
    // TODO -- make sure there's helpers for all the common things
    public abstract class HasLifecycle
    {
        private Lazy<ILifecycle> _lifecycle;

        protected void copyLifecycle(HasLifecycle other)
        {
            _lifecycle = other._lifecycle;
        }

        protected HasLifecycle()
        {
            _lifecycle =
                new Lazy<ILifecycle>(
                    () => { return scopedParent == null ? null : scopedParent.Lifecycle; });
        }

        protected HasLifecycle scopedParent { get; set; }

        /// <summary>
        /// Use InstanceScope for the constants now
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void SetLifecycleTo<T>() where T : ILifecycle, new()
        {
            _lifecycle = new Lazy<ILifecycle>(Lifecycles.Get<T>);
        }

        public void SetLifecycleTo(ILifecycle lifecycle)
        {
            _lifecycle = new Lazy<ILifecycle>(() => lifecycle);
        }

        public ILifecycle Lifecycle
        {
            get { return _lifecycle.Value; }
        }
    }
}