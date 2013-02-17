using System;
using StructureMap.Pipeline;

namespace StructureMap.Graph
{
    public abstract class HasScope
    {
        protected Lazy<ILifecycle> _lifecycle;

        protected HasScope()
        {
            _lifecycle = new Lazy<ILifecycle>(() => {
                return scopedParent == null ? Lifecycles.Transient : scopedParent.Lifecycle;
            });
        }

        protected HasScope scopedParent { get; set; }

        public void SetScopeTo(InstanceScope scope)
        {
            _lifecycle = new Lazy<ILifecycle>(() => Lifecycles.GetLifecycle(scope));
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