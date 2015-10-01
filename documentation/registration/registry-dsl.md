<!--Title: Registry DSL-->
<!--Url: registry-dsl-->

Creating `Registry` classes is the recommended way of using the Registry DSL. 

The Registry DSL is mostly a [fluent interface][1] with some nested [closure][2] 
usage. The intent of the Registry DSL is to make the configuration process as 
error free as possible by using "compiler safe" expressions and defensive 
programming to point out missing data.

## The Registry Class

On all but the smallest systems, the main unit of configuration will probably be 
the `Registry` class.  Typically, you would subclass the `Registry` class, then 
use the [fluent interface](https://en.wikipedia.org/wiki/Fluent_interface) methods exposed by the Registry class to create Container 
configuration. Here's a sample `Registry` class below used to configure an 
instance of an `IWidget` interface:

<[sample:simple-registry]>

## Including Registries

The next question is "how does my new `Registry` class get used?" 

When you set up a `Container`, you need to simply direct the 
`Container` to use the configuration in that `Registry` class:

<[sample:including-registries]>

## Named Instances

When you have multiple implementations of an interface, it can often be useful to
name instances. To retrieve a specific implementation:

<[sample:named-instance]>

You can also register named instances with the following shorthand:

<[sample:named-instances-shorthand]>

[1]: http://martinfowler.com/bliki/FluentInterface.html
[2]: http://en.wikipedia.org/wiki/Closure_%28computer_programming%29

