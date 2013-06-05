using System;
using StructureMap.Pipeline;

namespace StructureMap.Graph
{

    // TODO -- make sure there's helpers for all the common things
    public abstract class HasScope
    {
        private Lazy<ILifecycle> _lifecycle;

        protected void copyLifecycle(HasScope other)
        {
            _lifecycle = other._lifecycle;
        }

        protected HasScope()
        {
            _lifecycle = new Lazy<ILifecycle>(() => {
                return scopedParent == null ? Lifecycles.Transient : scopedParent.Lifecycle;
            });
        }

        protected HasScope scopedParent { get; set; }

        /// <summary>
        /// Use InstanceScope for the constants now
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void SetScopeTo<T>() where T: ILifecycle, new()
        {
            _lifecycle = new Lazy<ILifecycle>(() => Lifecycles.Get<T>());
        }

        public void SetScopeTo(ILifecycle lifecycle)
        {
            _lifecycle = new Lazy<ILifecycle>(() => lifecycle);
        }

        public ILifecycle Lifecycle
        {
            get
            {
                return _lifecycle.Value;
            }
        }
    }
}