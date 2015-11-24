<!--Title: Constructor Selection-->
<!--Url: constructor-selection-->


StructureMap 3.* greatly improves the control over the selection of constructor functions to build concrete types
by allowing users to override the constructor selection on specific _Instance's_ and register custom rules for
selecting constructors to override the basic StructureMap behavior. 

<div class="alert alert-info" role="alert">In all cases, StructureMap can only use <b>public</b> constructor functions.</div>

## Greediest Constructor

If there are multiple public constructor functions on a concrete class, StructureMap's default behavior is to 
select the "greediest" constructor, i.e., the constructor function with the most parameters. In the case of two or more
constructor functions with the same number of parameters StructureMap will simply take the first constructor encountered
in that subset of constructors.

The default constructor selection is demonstrated below:

<[sample:select-the-greediest-ctor]>

**New in StructureMap 4.0**, the "greediest constructor selection" will bypass any constructor function that requires "simple" arguments
like strings, numbers, or enumeration values that are not explicitly configured for the instance.

You can see this behavior shown below:

<[sample:skip-ctor-with-missing-simples]>


## Explicitly Selecting a Constructor
To override the constructor selection explicitly on a case by case basis, you
can use the `SelectConstructor(Expression)` method in the <[linkto:registration/registry-dsl]>
as shown below:

<[sample:explicit-ctor-selection]>

## [DefaultConstructor] Attribute

Alternatively, you can override the choice of constructor function by using the 
older `[DefaultConstructor]` attribute like this:

<[sample:using-default-ctor-attribute]>

## Custom Constructor Selection Rule

If the constructor selection logic isn't what you want, you can systematically 
override the selection rules by registering one or more custom `IConstructorSelector`
policies to a `Container`. Do note that you can register more than one policy and they
will be executed from last one registered to the first one registered. 

Let's say that you want to control the constructor selection for all concrete 
subclasses of an abstract class like this hiearchy:

<[sample:custom-ctor-scenario]>

You could create a custom `IConstructorSelector` like this one below to override
the constructor behavior for only subclasses of `BaseThing`:

<[sample:custom-ctor-rule]>

Finally, you can register your custom rule as shown below:

<[sample:using-custom-ctor-rule]>


