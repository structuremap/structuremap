using System;
using System.Runtime.ExceptionServices;

namespace StructureMap.DynamicInterception
{
    internal class MethodInvocationResult : IMethodInvocationResult
    {
        private readonly ExceptionDispatchInfo _exceptionDispatchInfo;

        public MethodInvocationResult(object returnValue)
        {
            ReturnValue = returnValue;
        }

        public MethodInvocationResult(Exception exception)
        {
            _exceptionDispatchInfo = ExceptionDispatchInfo.Capture(exception);
        }

        public bool Successful
        {
            get { return _exceptionDispatchInfo != null; }
        }

        public object ReturnValue { get; private set; }

        public Exception Exception
        {
            get
            {
                return _exceptionDispatchInfo != null ? _exceptionDispatchInfo.SourceException : null;
            }
        }

        public object GetReturnValueOrThrow()
        {
            if (_exceptionDispatchInfo != null)
            {
                _exceptionDispatchInfo.Throw();
            }

            return ReturnValue;
        }
    }
}