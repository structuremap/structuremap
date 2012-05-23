using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Interceptor;

namespace StructureMap.AutoFactory
{
    public class FactoryInterceptor : IInterceptor
    {
        private readonly IContext _context;
        private IContainer _container;

        private IContainer Container
        {
            get { return _container ?? (_container = _context.GetInstance<IContainer>()); }
        }

        public FactoryInterceptor(IContext context)
        {
            _context = context;
        }

        public void Intercept(IInvocation invocation)
        {
            Type pluginType;

            int remainingArguments = invocation.Arguments.Length;
            if ((invocation.Arguments.Length > 0) && (invocation.Arguments[0] is Type))
            {
                pluginType = (Type) invocation.Arguments[0];
                remainingArguments--;
            }
            else
            {
                pluginType = invocation.Method.ReturnType;                
            }

            var returnValue = GetReturnValue(invocation, pluginType, remainingArguments);
            invocation.ReturnValue = returnValue;
        }

        private object GetReturnValue(IInvocation invocation, Type pluginType, int remainingArguments)
        {
            object returnValue;
            if (remainingArguments > 0)
            {
                var i = invocation.Arguments.Length - remainingArguments;
                returnValue = GetReturnValueForMultipleArguments(invocation, pluginType, i, Container);
            }
            else
                returnValue = DoServiceLocator(pluginType, _context.GetInstance, t => Container.GetAllInstances(t));
            
            return returnValue;
        }

        private static object GetReturnValueForMultipleArguments(IInvocation invocation, Type pluginType, int i,
                                                                 IContainer container)
        {
            var parameters = invocation.Method.GetParameters();
            var name = parameters[i].Name;
            var value = invocation.Arguments[i];
            var expr = container.With(name).EqualTo(value);

            for (int k = i + 1; k < invocation.Arguments.Length; k++)
            {
                name = parameters[k].Name;
                value = invocation.Arguments[k];
                expr = expr.With(name).EqualTo(value);
            }

            return DoServiceLocator(pluginType, expr.GetInstance, expr.GetAllInstances);
        }

        private static object DoServiceLocator(Type pluginType, Func<Type, object> singleLocator, Func<Type, object> multipleLocator)
        {
            if (IsRequestForAllComponents(pluginType))
            {
                pluginType = GetEnumerableType(pluginType);
                var list = (IList)multipleLocator(pluginType);
                return ConvertToTypedList(pluginType, list);
            }

            return singleLocator(pluginType);
        }

        private static Array ConvertToTypedList(Type pluginType, IList list)
        {
            var array = Array.CreateInstance(pluginType, list.Count);
            for (int i = 0; i < array.Length; i++)
                array.SetValue(list[i], i);
            return array;
        }

        private static Type GetEnumerableType(Type pluginType)
        {
            var argments = pluginType.GetGenericArguments();
            pluginType = argments[0];
            return pluginType;
        }

        private static bool IsRequestForAllComponents(Type pluginType)
        {
            return pluginType.IsGenericType 
                && typeof(IEnumerable).IsAssignableFrom(pluginType);
        }
    }
}