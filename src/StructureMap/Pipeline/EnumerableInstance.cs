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

        public static IEnumerable<Type> OpenEnumerableTypes
        {
            get
            {
                return _enumerableTypes;
            }
        }

        public static Type DetermineElementType(Type pluginType)
        {
            if (pluginType.IsArray)
            {
                return pluginType.GetElementType();
            }

            return pluginType.GetGenericArguments().First();
        }

        private readonly Type _propertyType;
        private readonly IEnumerable<Instance> _children;
        private readonly IEnumerableCoercion _coercion;
        private readonly string _description;

        public EnumerableInstance(Type propertyType, IEnumerable<Instance> children)
        {
            _description = propertyType.FullName;

            _propertyType = propertyType;
            _children = children;
            _coercion = DetermineCoercion(propertyType);
        }

        public IEnumerable<Instance> Children { get { return _children; } }

        public static IEnumerableCoercion DetermineCoercion(Type propertyType)
        {
            Type coercionType = determineCoercionType(propertyType);
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
                Type templateType = propertyType.GetGenericTypeDefinition();
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
            var elementType = _coercion.ElementType;

            if (!_children.Any())
            {
                return new AllPossibleValuesDependencySource(_propertyType);
            }

            var items = _children.Select(x => x.ToDependencySource(elementType)).ToArray();

            if (_propertyType.IsArray)
            {
                return new ArrayDependencySource(elementType, items);
            }

            var parentType = _propertyType.GetGenericTypeDefinition();
            return parentType == typeof (List<>) 
                ? new ListDependencySource(elementType, items) 
                : new ArrayDependencySource(elementType, items);
        }

        protected override string getDescription()
        {
            return _description;
        }

        protected override object build(Type pluginType, BuildSession session)
        {
            Type elementType = _coercion.ElementType;

            IEnumerable<object> objects = buildObjects(elementType, session);
            return _coercion.Convert(objects);
        }

        private IEnumerable<object> buildObjects(Type elementType, BuildSession session)
        {
            if (_children == null)
            {
                return session.GetAllInstances(elementType);
            }

            return _children.Select(x => x.Build(elementType, session));
        }

        public static bool IsEnumerable(Type type)
        {
            if (type.IsArray) return true;

            return type.IsGenericType && type.GetGenericTypeDefinition().IsIn(_enumerableTypes);
        }


    }
}