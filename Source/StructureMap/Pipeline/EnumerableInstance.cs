using System;
using System.Collections.Generic;
using System.Linq;

namespace StructureMap.Pipeline
{
    public class EnumerableInstance : Instance
    {
        private readonly IEnumerable<Instance> _children;
        private readonly IEnumerableCoercion _coercion;
        private readonly string _description;

        public EnumerableInstance(Type propertyType, IEnumerable<Instance> children)
        {
            _description = propertyType.FullName;

            _children = children;
            _coercion = DetermineCoercion(propertyType);
        }

        public static IEnumerableCoercion DetermineCoercion(Type propertyType)
        {
            var coercionType = determineCoercionType(propertyType);
            return (IEnumerableCoercion) Activator.CreateInstance(coercionType);
        }

        private static readonly List<Type> _enumerableTypes = new List<Type>(){typeof(IEnumerable<>), typeof(IList<>), typeof(List<>)};
        private static Type determineCoercionType(Type propertyType)
        {
            if (propertyType.IsArray)
            {
                return typeof (ArrayCoercion<>).MakeGenericType(propertyType.GetElementType());
            }

            if (propertyType.IsGenericType )
            {
                var templateType = propertyType.GetGenericTypeDefinition();
                if (_enumerableTypes.Contains(templateType))
                {
                    return typeof (ListCoercion<>).MakeGenericType(propertyType.GetGenericArguments().First());
                }
            }

            throw new ArgumentException("Only IEnumerable<T> types can be passed to this constructor.  {0} is invalid".ToFormat(propertyType.AssemblyQualifiedName));
        }

        protected override string getDescription()
        {
            return _description;
        }

        protected override object build(Type pluginType, BuildSession session)
        {
            var objects = _children.Select(x => x.Build(pluginType, session));
            return _coercion.Convert(objects);
        }
    }
}