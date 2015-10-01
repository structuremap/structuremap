<!--Title: Setter Injection-->
<!--Url: setter-injection-->


<div class="alert alert-info" role="alert">In all cases, <i>Setter Injection</i> is an opt-in feature in StructureMap that has to be explicitly enabled
	on a case by case basis.</div>

StructureMap can inject dependencies into public setter properties as part of its construction process using the _Setter Injection_ form of Dependency Injection. The StructureMap team strongly recommends using constructor injection wherever possible instead of setter injection. That being said,
there are few cases where setter injection is probably easier (inheritance hierarchies), not to mention legacy or third party tools that
simply cannot support constructor injection *cough* ASP.Net *cough*.

See this discussion from Martin Fowler on [Constructor vs Setter Injection](http://martinfowler.com/articles/injection.html#ConstructorVersusSetterInjection).

**If you are having any trouble with setter injection in your StructureMap usage, make sure you're familiar with using <[linkto:diagnostics/build-plans]>
to help in troubleshooting**


## Explicit Setter Injection with [SetterProperty] Attributes

The simplest conceptual way to force StructureMap into making public setters mandatory service dependencies by decorating setter properties with the `[SetterProperty]` attribute like this example:

<[sample:setter-injection-with-SetterProperty]>

Without the `[SetterProperty]` attributes decorating the setters, StructureMap would ignore the `Provider` and `ShouldCache` properties when it builds up a `Repository` object. With the attributes, StructureMap will try to build and attach values for the two properties as part of object construction.

If you were to look at StructureMap's "build plan" for the `Repository` class, you would see something like this:

<pre>
PluginType: StructureMap.Testing.DocumentationExamples.Repository
Lifecycle: Transient
new Repository()
    Set IDataProvider Provider = **Default**
    Set Boolean ShouldCache = Value: False
</pre>

If you intensely dislike runaway attribute usage, that's okay because there are other ways to enable setter injection in StructureMap.

## Inline Setter Configuration

Any setter property not configured with `[SetterProperty]` or the setter policies in the next section can still be filled by StructureMap if an inline dependency is configured matching that setter property as shown in the example below:

<[sample:inline-dependencies-setters]>

See also:

* <[linkto:registration/inline-dependencies]>
* <[linkto:registration/configured-instance]>


## Setter Injection Policies

Lastly, you can give StructureMap some criteria for determining which setters should be mandatory dependencies with the `Registry.Policies.SetAllProperties()` method during Container setup as shown in this example below:

<[sample:using-setter-policy]>

All calls to `Registry.Policies.SetAllProperties()` are additive, meaning you can use as many criteria as possible for setter injection.




