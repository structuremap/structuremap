using StructureMap.Construction;
using StructureMap.Util;

namespace StructureMap.Testing.Construction
{
    public class StubArguments : IArguments
    {
        private readonly Cache<string, object> _values = new Cache<string, object>();

        public T Get<T>(string propertyName)
        {
            return (T) _values[propertyName];
        }

        public bool Has(string propertyName)
        {
            return _values.Has(propertyName);
        }

        public void Set(string propertyName, object value)
        {
            _values[propertyName] = value;
        }
    }
}