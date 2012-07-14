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
            int remainingArguments;
            var pluginType = DetermineReturnType(invocation, out remainingArguments);

            var returnValue = GetReturnValue(invocation, pluginType, remainingArguments);
            invocation.ReturnValue = returnValue;
        }

        private static Type DetermineReturnType(IInvocation invocation, out int remainingArguments)
        {
            remainingArguments = invocation.Arguments.Length;
            if ((invocation.Arguments.Length > 0) && (invocation.Arguments[0] is Type))
            {
                remainingArguments--;
                return (Type) invocation.Arguments[0];
            }

            return invocation.Method.ReturnType;
        }

        private object GetReturnValue(IInvocation invocation, Type pluginType, int remainingArguments)
        {
            if (remainingArguments > 0)
            {
                var i = invocation.Arguments.Length - remainingArguments;
                return GetReturnValueForMultipleArguments(invocation, pluginType, i, Container);
            }

            return DoServiceLocator(pluginType, _context.GetInstance, t => Container.GetAllInstances(t));
        }

        private static object GetReturnValueForMultipleArguments(IInvocation invocation, Type pluginType, int i,
                                                                 IContainer container)
        {
            var parameters = invocation.Method.GetParameters();
            var childContainer = container.GetNestedContainer();
            childContainer.Configure(conf =>
                {
                    for (int k = i; k < invocation.Arguments.Length; k++)
                    {
                        conf.For(parameters[k].ParameterType).Use(invocation.Arguments[k])
                            .Named(parameters[k].Name);
                    }
                });

            return DoServiceLocator(pluginType, childContainer.GetInstance, childContainer.GetAllInstances);
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