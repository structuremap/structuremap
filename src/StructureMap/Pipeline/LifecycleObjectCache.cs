using System;
using System.Collections.Generic;
using System.Threading;

namespace StructureMap.Pipeline
{
    [Serializable]
    public class LifecycleObjectCache : IObjectCache
    {
        private readonly ReaderWriterLockSlim _lock;
        private readonly IDictionary<int, object> _objects = new Dictionary<int, object>();

        public LifecycleObjectCache()
        {
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        public int Count
        {
            get { return _lock.Read(() => _objects.Count); }
        }

        public bool Has(Type pluginType, Instance instance)
        {
            return _lock.Read(() => {
                int key = instance.InstanceKey(pluginType);
                return _objects.ContainsKey(key);
            });
        }

        public void Eject(Type pluginType, Instance instance)
        {
            int key = instance.InstanceKey(pluginType);
            _lock.MaybeWrite(() => {
                if (!_objects.ContainsKey(key)) return;

                _lock.Write(() => {
                    var disposable = _objects[key] as IDisposable;
                    _objects.Remove(key);
                    disposable.SafeDispose();
                });
            });
        }

        public object Get(Type pluginType, Instance instance, IBuildSession session)
        {
            object result = null;
            int key = instance.InstanceKey(pluginType);
            _lock.EnterUpgradeableReadLock();
            if (_objects.ContainsKey(key))
            {

                result = _objects[key];
                _lock.ExitUpgradeableReadLock();
            }
            else
            {
                _lock.EnterWriteLock();
                try
                {
                    result = session.BuildNewInSession(pluginType, instance);
                    _objects.Add(key, result);
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }

            return result;
        }

        public void DisposeAndClear()
        {
            _lock.Write(() => {
                _objects.Values.Each(@object => {
                    if (@object is Container) return;

                    if (@object != null) @object.SafeDispose();
                });

                _objects.Clear();
            });
        }


        public void Set(Type pluginType, Instance instance, object value)
        {
            if (value == null) return;

            _lock.Write(() => {
                int key = instance.InstanceKey(pluginType);
                if (_objects.ContainsKey(key))
                {
                    _objects[key] = value;
                }
                else
                {
                    _objects.Add(key, value);
                }
            });
        }
    }

    public static class ReaderWriterLockExtensions
    {
        public static void Write(this ReaderWriterLockSlim rwLock, Action action)
        {
            rwLock.EnterWriteLock();
            try
            {
                action();
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public static T Read<T>(this ReaderWriterLockSlim rwLock, Func<T> func)
        {
            rwLock.EnterReadLock();
            try
            {
                return func();
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public static void MaybeWrite(this ReaderWriterLockSlim theLock, Action action)
        {
            try
            {
                theLock.EnterUpgradeableReadLock();
                action();
            }
            finally
            {
                theLock.ExitUpgradeableReadLock();
            }
        }

        public static T MaybeWrite<T>(this ReaderWriterLockSlim theLock, Func<T> answer, Func<bool> missingTest,
                                      Action write)
        {
            try
            {
                theLock.EnterUpgradeableReadLock();
                if (missingTest())
                {
                    theLock.Write(() => {
                        if (missingTest())
                        {
                            write();
                        }
                    });
                }

                return answer();
            }
            finally
            {
                if (theLock.IsReadLockHeld)
                {
                    theLock.ExitUpgradeableReadLock();
                }
            }
        }
    }
}