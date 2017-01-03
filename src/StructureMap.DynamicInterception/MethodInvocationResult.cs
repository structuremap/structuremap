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

        public bool Successful => _exceptionDispatchInfo == null;

        public object ReturnValue { get; }

        public Exception Exception => _exceptionDispatchInfo?.SourceException;

        public object GetReturnValueOrThrow()
        {
            _exceptionDispatchInfo?.Throw();

            return ReturnValue;
        }
    }
}