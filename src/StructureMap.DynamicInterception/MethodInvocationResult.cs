using System;

namespace StructureMap.DynamicInterception
{
    internal class MethodInvocationResult : IMethodInvocationResult
    {
        public MethodInvocationResult(object returnValue)
        {
            Successful = true;
            ReturnValue = returnValue;
        }

        public MethodInvocationResult(Exception exception)
        {
            Successful = false;
            Exception = exception;
        }

        public bool Successful { get; private set; }

        public object ReturnValue { get; private set; }

        public Exception Exception { get; private set; }
    }
}