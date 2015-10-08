<!--Title:Building Objects with Lambdas-->

Instead of allowing StructureMap to build objects directly, you can give a StructureMap `Container` a [Lambda function](https://msdn.microsoft.com/en-us/library/bb397687.aspx) that can be called to create an object at resolution time.

Using NHibernate's [`ISession`](https://github.com/nhibernate/nhibernate-core/blob/master/src/NHibernate/ISession.cs) as an example
of an object that typically has to be built by using an [`ISessionFactory`](https://github.com/nhibernate/nhibernate-core/blob/master/src/NHibernate/ISessionFactory.cs) object:

<[sample:nhibernate-isession-factory]>

If we want to allow StructureMap to control the `ISession` lifecycle and creation, we have to register a Lambda function as the 
means of creating `ISession` as shown in this example below:

<[sample:SessionFactoryRegistry]>

Lambda registrations can be done with any of the following four signatures:

1. `(Expression<Func<IContext, T>> builder)` -- a simple, one line Lambda to build `T` using `IContext`
1. `(Expression<Func<T>> func)` -- a simple, one line Lambda to build `T`
1. `(string description, Func<IContext, T> builder)` -- use `IContext` in your builder Lambda with a user-supplied description for diagnostics
1. `(string description, Func<T> builder)` -- Supply a complex `Func<T>` with a user-supplied description for diagnostics

**Be very wary of the difference between legal `Expression's` and more complicated Lambda's that will need to be `Func's`.** It likely doesn't matter to
you the user, but it unfortunately does to StructureMap and .Net itself. If you need to use a more complex `Func`, you will have
to supply a diagnostic description.


See <[linkto:the-container/working-with-the-icontext-at-build-time]> for more information.