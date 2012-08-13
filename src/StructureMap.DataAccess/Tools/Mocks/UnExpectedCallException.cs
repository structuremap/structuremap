using System;

namespace StructureMap.DataAccess.Tools.Mocks
{
    public class UnExpectedCallException : ApplicationException
    {
        public UnExpectedCallException(string name) : base("Too many calls to " + name)
        {
        }
    }
}