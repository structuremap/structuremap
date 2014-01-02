using System;
using System.Collections.Generic;
using StructureMap.TypeRules;

namespace StructureMap
{
    public interface OpenGenericTypeSpecificationExpression
    {
        T As<T>();
    }

    public interface OpenGenericTypeListSpecificationExpression
    {
        IList<T> As<T>();
    }

    public class CloseGenericTypeExpression : OpenGenericTypeSpecificationExpression,
        OpenGenericTypeListSpecificationExpression
    {
        private readonly IContainer _container;
        private readonly object _subject;
        private Type _pluginType;

        public CloseGenericTypeExpression(object subject, IContainer container)
        {
            _subject = subject;
            _container = container;
        }

        IList<T> OpenGenericTypeListSpecificationExpression.As<T>()
        {
            var list = _container.With(_subject.GetType(), _subject).GetAllInstances(_pluginType);
            var returnValue = new List<T>();
            foreach (var o in list)
            {
                returnValue.Add((T) o);
            }

            return returnValue;
        }

        /// <summary>
        /// specify what type you'd like the service returned as
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T OpenGenericTypeSpecificationExpression.As<T>()
        {
            return (T) _container.With(_subject.GetType(), _subject).GetInstance(_pluginType);
        }

        /// <summary>
        /// Specify the open generic type that should have a single generic parameter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public OpenGenericTypeSpecificationExpression GetClosedTypeOf(Type type)
        {
            closeType(type);
            return this;
        }

        private void closeType(Type type)
        {
            if (!type.IsOpenGeneric())
            {
                throw new StructureMapException(285);
            }

            _pluginType = type.MakeGenericType(_subject.GetType());
        }

        public OpenGenericTypeListSpecificationExpression GetAllClosedTypesOf(Type type)
        {
            closeType(type);
            return this;
        }
    }
}