using System;
using System.Collections;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap
{
    public interface IExplicitProperty
    {
        /// <summary>
        /// Specify the value of this explicit argument
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ExplicitArgsExpression EqualTo(object value);
    }

    public interface IExplicitArgsExpression
    {
        /// <summary>
        /// Pass in additional arguments by type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <returns></returns>
        ExplicitArgsExpression With<T>(T arg);

        /// <summary>
        /// Pass in additional arguments by type
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        ExplicitArgsExpression With(Type pluginType, object arg);

        /// <summary>
        /// Pass in additional arguments by name
        /// </summary>
        /// <param name="argName"></param>
        /// <returns></returns>
        IExplicitProperty With(string argName);
    }

    public class ExplicitArgsExpression : IExplicitProperty, IExplicitArgsExpression
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
            _args.Set(arg);
            return this;
        }

        /// <summary>
        /// Pass in additional arguments by type
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public ExplicitArgsExpression With(Type pluginType, object arg)
        {
            _args.Set(pluginType, arg);
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
        /// Gets the default instance of type T using the explicitly configured arguments from the "args"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetInstance<T>()
        {
            return _container.GetInstance<T>(_args);
        }

        /// <summary>
        /// Gets a named instance of type T using the explicitly configured arguments from teh "args"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetInstance<T>(string name)
        {
            return _container.GetInstance<T>(_args, name);
        }

        /// <summary>
        /// Gets the default instance of the pluginType using the explicitly configured arguments from the "args"
        /// </summary>
        /// <returns></returns>
        public object GetInstance(Type type)
        {
            return _container.GetInstance(type, _args);
        }

        /// <summary>
        /// Gets the default instance of the pluginType using the explicitly configured arguments from the "args"
        /// </summary>
        /// <returns></returns>
        public object GetInstance(Type type, string name)
        {
            return _container.GetInstance(type, _args, name);
        }


        /// <summary>
        /// Gets all configured instances of type T using explicitly configured arguments
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetAllInstances<T>()
        {
            return _container.GetAllInstances<T>(_args);
        }

        public IEnumerable GetAllInstances(Type type)
        {
            return _container.GetAllInstances(type, _args);
        }
    }
}