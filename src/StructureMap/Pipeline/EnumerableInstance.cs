using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Building;

namespace StructureMap.Pipeline
{
    public class EnumerableInstance : Instance
    {
        private static readonly List<Type> _enumerableTypes = new List<Type>
        {
            typeof (IEnumerable<>),
            typeof (IList<>),
            typeof (List<>)
        };

        public override Type ReturnedType
        {
            get { return null; }
        }

        public static IEnumerable<Type> OpenEnumerableTypes
        {
            get { return _enumerableTypes; }
        }

        public static Type DetermineElementType(Type pluginType)
        {
            if (pluginType.IsArray)
            {
                return pluginType.GetElementType();
            }

            return pluginType.GetGenericArguments().First();
        }

        private readonly IEnumerable<Instance> _children;

        public EnumerableInstance(IEnumerable<Instance> children)
        {
            _children = children;
        }

        public IEnumerable<Instance> Children
        {
            get { return _children; }
        }

        public static IEnumerableCoercion DetermineCoercion(Type propertyType)
        {
            var coercionType = determineCoercionType(propertyType);
            return (IEnumerableCoercion) Activator.CreateInstance(coercionType);
        }

        private static Type determineCoercionType(Type propertyType)
        {
            if (propertyType.IsArray)
            {
                return typeof (ArrayCoercion<>).MakeGenericType(propertyType.GetElementType());
            }

            if (propertyType.IsGenericType)
            {
                var templateType = propertyType.GetGenericTypeDefinition();
                if (_enumerableTypes.Contains(templateType))
                {
                    return typeof (ListCoercion<>).MakeGenericType(propertyType.GetGenericArguments().First());
                }
            }

            throw new ArgumentException(
                "Only IEnumerable<T> types can be passed to this constructor.  {0} is invalid".ToFormat(
                    propertyType.AssemblyQualifiedName));
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            var coercion = DetermineCoercion(pluginType);
            var elementType = coercion.ElementType;

            if (!_children.Any())
            {
                return new AllPossibleValuesDependencySource(pluginType);
            }

            var items = _children.Select(x => x.ToDependencySource(elementType)).ToArray();

            if (pluginType.IsArray)
            {
                return new ArrayDependencySource(elementType, items);
            }

            var parentType = pluginType.GetGenericTypeDefinition();
            return parentType == typeof (List<>)
                ? new ListDependencySource(elementType, items)
                : new ArrayDependencySource(elementType, items);
        }

        protected override string getDescription()
        {
            return "Enumerable Instance";
        }

        public static bool IsEnumerable(Type type)
        {
            if (type.IsArray) return true;

            return type.IsGenericType && type.GetGenericTypeDefinition().IsIn(_enumerableTypes);
        }
    }
}