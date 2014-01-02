using System;
using System.Collections.Generic;
using StructureMap.Configuration.DSL.Expressions;

namespace StructureMap.Pipeline
{
    /// <summary>
    ///     Expression Builder to help define multiple Instances for an Array dependency
    /// </summary>
    /// <typeparam name="TElementType"></typeparam>
    /// <typeparam name="TInstance"></typeparam>
    public class ArrayDefinitionExpression<TInstance, TElementType> where TInstance : ConstructorInstance
    {
        private readonly TInstance _instance;
        private readonly string _propertyName;

        internal ArrayDefinitionExpression(TInstance instance, string propertyName)
        {
            _instance = instance;
            _propertyName = propertyName;
        }

        /// <summary>
        ///     Nested Closure that allows you to add an unlimited number of child Instances
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public TInstance Contains(Action<IInstanceExpression<TElementType>> action)
        {
            var list = new List<Instance>();

            var child = new InstanceExpression<TElementType>(list.Add);
            action(child);

            _instance.Dependencies.Add(_propertyName, typeof(IEnumerable<TElementType>), new EnumerableInstance(list));

            return _instance;
        }

        /// <summary>
        ///     Specify an array of Instance objects directly for an Array dependency
        /// </summary>
        /// <param name="children"></param>
        /// <returns></returns>
        public TInstance Contains(params Instance[] children)
        {
            _instance.Dependencies.Add(_propertyName, typeof(IEnumerable<TElementType>), new EnumerableInstance(children));

            return _instance;
        }
    }
}