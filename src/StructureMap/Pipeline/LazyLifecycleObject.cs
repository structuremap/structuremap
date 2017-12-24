using System;
using System.Diagnostics;
using System.Threading;

namespace StructureMap.Pipeline
{
    [DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={ValueForDebugView}")]
    [DebuggerTypeProxy(typeof(LazyLifecycleObjectDebugView<>))]
    public class LazyLifecycleObject<T>
    {
        private static readonly object _usedThreadSafeObj = new object();
        private static readonly Func<T> _usedValueFactory = () => throw new InvalidOperationException("Value was already created.");

        private Boxed _boxed;
        private object _threadSafeObj;
        private Func<T> _valueFactory;

        public bool IsValueCreated
        {
            get
            {
                return _boxed != null;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public T Value
        {
            get
            {
                return IsValueCreated ? _boxed.Value : CreateValue();
            }
        }

        internal T ValueForDebugView
        {
            get
            {
                return IsValueCreated ? _boxed.Value : default(T);
            }
        }

        public LazyLifecycleObject(Func<T> valueFactory)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }

            _threadSafeObj = new object();
            _valueFactory = valueFactory;
        }

        public override string ToString()
        {
            return IsValueCreated ? Value.ToString() : "Value is not created.";
        }

        private T CreateValue()
        {
            var threadSafeObj = Volatile.Read(ref _threadSafeObj);
            var lockTaken = false;

            try
            {
                if (threadSafeObj != _usedThreadSafeObj)
                {
                    Monitor.Enter(threadSafeObj, ref lockTaken);

                    if (_boxed == null)
                    {
                        _boxed = new Boxed(_valueFactory());
                        _valueFactory = _usedValueFactory;

                        Volatile.Write(ref _threadSafeObj, _usedThreadSafeObj);
                    }
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(threadSafeObj);
                }
            }

            return _boxed.Value;
        }

        private class Boxed
        {
            internal readonly T Value;

            internal Boxed(T value)
            {
                Value = value;
            }
        }
    }

    internal class LazyLifecycleObjectDebugView<T>
    {
        private readonly LazyLifecycleObject<T> _lazy;

        public LazyLifecycleObjectDebugView(LazyLifecycleObject<T> lazy)
        {
            _lazy = lazy;
        }
        
        public bool IsValueCreated
        {
            get
            {
                return _lazy.IsValueCreated;
            }
        }
        
        public T Value
        {
            get
            {
                return _lazy.ValueForDebugView;
            }
        }
    }
}