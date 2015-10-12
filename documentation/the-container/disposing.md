<!--Title:StructureMap and IDisposable-->



## Singletons

This one is easy, any object marked as the _Singleton_ lifecycle that implements `IDisposable` is disposed when the root container is
disposed:

<[sample:singleton-in-action]>

_Ejecting_ a singleton-scoped object will also force it to be disposed. See <[linkto:diagnostics/using-the-container-model]> for more information on how to eject singletons
from a running `Container`.


## Nested Containers

As discussed in <[linkto:the-container/nested-containers]>, any transient or container-scoped object that implements `IDisposable` and is created
by a nested container will be disposed as the nested container is disposed:

<[sample:nested-disposal]>


## Transients

**Regardless of the new tracking/release mode, the StructureMap team still strongly recommends using a nested container per HTTP request or service
bus message handling session or logical transaction to deal with disposing transient objects.**

By default, StructureMap will **not** hang on to any transient-scoped objects created by the root or child containers. Dealing with
`IDisposable` is completely up to the user in this case. The StructureMap team has long believed that trying to track transient disposables with
an explicit `Release(object)` mode as other IoC containers behave would do more harm than good (memory leaks from forgetting to call Release(), more work on the user). 

That all being said, in order to comply with the new ASP.Net vNext compliance behavior, StructureMap 4.0 introduces a new opt-in transient tracking mode with the prerequisite `Release(object)` method:

<[sample:transient_tracking_mode]>

**As of right now, StructureMap will only track the top level object requested from a `Container` and not all the other `IDisposable` objects that may
have been created as part of the main object graph.**

