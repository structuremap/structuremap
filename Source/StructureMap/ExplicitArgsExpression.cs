using System;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap
{
    public interface IExplicitProperty
    {
        ExplicitArgsExpression EqualTo(object value);
    }

    public class ExplicitArgsExpression : IExplicitProperty
    {
        private readonly ExplicitArguments _args = new ExplicitArguments();
        private readonly IContainer _container;
        private string _lastArgName;

        internal ExplicitArgsExpression(IContainer container)
        {
            _container = container;
        }

        #region IExplicitProperty Members

        ExplicitArgsExpression IExplicitProperty.EqualTo(object value)
        {
            _args.SetArg(_lastArgName, value);
            return this;
        }

        #endregion

        /// <summary>
        /// Pass in additional arguments by type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        public ExplicitArgsExpression With<T>(T arg)
        {
            _args.Set<T>(arg);
            return this;
        }

        /// <summary>
        /// Pass in additional arguments by name
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        public IExplicitProperty With(string argName)
        {
            _lastArgName = argName;
            return this;
        }

        /// <summary>
        /// Create an instance using the explicit arguments
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetInstance<T>()
        {
            return _container.GetInstance<T>(_args);
        }

        /// <summary>
        /// Create an instance using the explicit arguments
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object GetInstance(Type type)
        {
            return _container.GetInstance(type, _args);
        }

        public IList<T> GetAllInstances<T>()
        {
            return _container.GetAllInstances<T>(_args);
        }
    }
}