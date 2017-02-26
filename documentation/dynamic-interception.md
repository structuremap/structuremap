<!--title:Aspect Oriented Programming with StructureMap.DynamicInterception-->
<div class="alert alert-info" role="alert">You need to install <a href="https://www.nuget.org/packages/StructureMap.DynamicInterception">StructureMap.DynamicInterception</a> package to use the functionality described below.</div>

The <[linkto:interception-and-decorators;title=Interception and Decorators]> page describes how to use "static" interception of created instances, mainly allowing to apply [the Decorator pattern](https://en.wikipedia.org/wiki/Decorator_pattern). It is "static" in a sense that you need know what interfaces you want to implement and you need to create decorators implementing all appropriate interfaces. It is almost always fine, but there are cases when you want to implement the same decorating logic for many interfaces which easily breaks [the DRY principle](https://en.wikipedia.org/wiki/Don%27t_repeat_yourself). The most common examples are [cross-cutting concerns](https://en.wikipedia.org/wiki/Cross-cutting_concern) such as logging, caching etc.

## Dynamic proxy interceptor
Let's see it in action. Let's say we have 
<[sample:IMathService]>
with implementation
<[sample:MathService]>

If you need to apply dynamic interception to `IMathService`, first you implement either `ISyncInterceptionBehavior` or `IAsyncInterceptionBehavior` depending if your interception code needs to use `async`/`await` or not. For demonstration purposes, let's have 
<[sample:NegatingInterceptor]>
and
<[sample:AsyncCachingInterceptor]>

Finally, we register interceptors with help of `DynamicProxyInterceptor` as follows:
<[sample:CallSyncMethodWithSyncThenAsyncInterceptors]>

The idea is simple  - for each method call of the intercepted instance, `Intercept`/`InterceptAsync` is called passing an `IMethodInvocation` instance allowing to get information about the intercepted method and modify arguments if needed before passing to the next interceptor. You have a choice to call the next interceptor in the chain via `methodInvocation.InvokeNext`/`methodInvocation.InvokeNextAsync`. Alternatively, you can return the result directly from the interceptor via `methodInvocation.CreateResult`.

Finally, you can also throw exceptions from interceptors either directly or by returning `methodInvocation.CreateExceptionResult`.

## Dynamic proxy policy
As described on the <[linkto:interception-and-decorators;title=Interception and Decorators]> page, you can create an interception policy if you need to apply interceptors to many types by certain filter. `DynamicProxyInterceptorPolicy` makes it easier when it comes to dynamic interceptors. See the example below:
<[sample:UseInterceptionPolicy]>

Check `DynamicProxyInterceptorPolicy` constructors to find the most suitable overload for your purposes.