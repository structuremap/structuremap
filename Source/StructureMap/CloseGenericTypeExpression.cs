using System;

namespace StructureMap
{
    public interface OpenGenericTypeSpecificationExpression
    {
        T As<T>();
    }

    public class CloseGenericTypeExpression : OpenGenericTypeSpecificationExpression
    {
        private readonly object _subject;
        private readonly IContainer _container;
        private Type _pluginType;

        public CloseGenericTypeExpression(object subject, IContainer container)
        {
            _subject = subject;
            _container = container;
        }

        /// <summary>
        /// Specify the open generic type that should have a single generic parameter
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public OpenGenericTypeSpecificationExpression GetClosedTypeOf(Type type)
        {
            if (!type.IsGeneric())
            {
                throw new StructureMapException(285);
            }

            _pluginType = type.MakeGenericType(_subject.GetType());
            return this;
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
    }
}