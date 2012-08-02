using System;
using System.Linq;
using System.Reflection;

namespace StructureMap.AutoMocking
{
    public class SubstituteFactory
    {
        private readonly Func<Type, object> _factory;

        public SubstituteFactory()
        {
            var assembly = Assembly.Load("NSubstitute");
            var type = assembly.GetType("NSubstitute.Substitute");
            
            if (type == null)
                throw new InvalidOperationException("Can't find Substitute class in assembly @ " + assembly.Location);

            var method = type.GetMethods().First(x => x.ContainsGenericParameters && x.GetGenericArguments().Length == 1);

            _factory = typeToMock => method.MakeGenericMethod(typeToMock).Invoke(null, new object[1]{null});
        }

        public object CreateMock(Type type)
        {
            return _factory(type);
        }
    }
}