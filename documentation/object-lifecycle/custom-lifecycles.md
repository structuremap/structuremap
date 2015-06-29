<!--Title: Custom Lifecycles-->
<!--Url: custom-lifecycles-->


Lifecycle's in StructureMap are pluggable.  To create your own custom lifecycle, make an implementation of the <code>ILifecycle</code> interface like this one:

<[sample:CustomLifecycle]>

Registering your custom lifecycle can be done like this:

<[sample:using-custom-lifecycle]>

Most of the built in <code>ILifecycle</code> implementations use <code>LifecycleObjectCache</code> internally, but you may also need to create your own <code>IObjectCache</code> implementation.

