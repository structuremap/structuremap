using System;
using System.Collections.Generic;
using System.Linq;

namespace StructureMap.Pipeline
{
    public class TrackingTransientCache : ITransientTracking
    {
        private readonly IList<object> _tracked = new List<object>();
        public TransientTracking Style
        {
            get
            {
                return TransientTracking.ExplicitReleaseMode;
            }
        }

        public object Get(Type pluginType, Instance instance, IBuildSession session)
        {
            var @object = session.BuildNewInSession(pluginType, instance);
            if (@object is IDisposable)
            {
                _tracked.Add(@object);
            }

            return @object;
        }

        public int Count
        {
            get { return 0; }
        }

        public bool Has(Type pluginType, Instance instance)
        {
            return false;
        }

        public void Eject(Type pluginType, Instance instance)
        {
            // Nullo
        }

        public void DisposeAndClear()
        {
            var tracked = _tracked.ToArray();
            _tracked.Clear();

            // We want this to throw exceptions here
            tracked.OfType<IDisposable>().Each(x => x.Dispose());
        }

        public void Release(object o)
        {
            if (_tracked.Remove(o))
            {
                if (o is IDisposable)
                {
                    o.As<IDisposable>().Dispose();
                }
            }
        }

        public IEnumerable<object> Tracked
        {
            get { return _tracked; }
        }
    }
}