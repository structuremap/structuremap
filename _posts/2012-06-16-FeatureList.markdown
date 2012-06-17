---
title: Feature List
layout: default
---

StructureMap includes all of the basic capabilities of an IoC tool, plus much,
much more:


* Creates objects using the [Constructor Injection
pattern](http://picocontainer.codehaus.org/Constructor+Injection) Constructor
function selection can be overriden programmatically or with an attribute

  * Constructor function selection can be overriden programmatically or with an
    attribute

* Optionally, attach dependencies and other properties through Setter Injection. 
Properties can be configured to be mandatory dependencies or optional
dependencies.Convention based setter injection policies

  * Convention based setter injection policies

* Runtime linking of object dependencies with auto wiring
* "Build Up" using setter injection to attach dependencies to an externally
constructed object

* Can be configured to create or resolve objects by:Calling constructor
functionsLambda expressionsCloning a prototype objectLoading a UserControlUUsing
an externally created object Conditional Construction of objectsCustom method of
construction objects

  * Calling constructor functions
  * Lambda expressions
  * Cloning a prototype object
  * Loading a UserControl
  * Using an externally created object
  *  Conditional Construction of objects
  * Custom method of construction objects

* Object graphs can be configured inline to override auto wiringLI>Contextual
construction of objects

* "Missing Instance" handling
* Passing explicit arguments into the Container
* Configurable object [lifecycle scoping](Scoping.htm) (singleton, transient,
HttpContext, ThreadLocal)

*  Auto registration with pluggable type convention rules
* Generalized support for the [Plugin](Concepts.htm#Plugin) pattern using (almost)
any type of class or interface

* Configurable either implicitly through a programmatic DSL, [custom
attributes](UsingAttributes.htm), or explicitly through XML configuration

* Very modular configuration options.  Mix and match any form of configuration at
one time.

* Configuration can be added at runtime
* Supports type registration of open generic types
* Interception capabilities to apply runtime AOP or decorators
* Extensible object creation
* Mock Injection with Containers 
* Runtime injection of a static mock or stub

* An Auto Mocking Container that can be used with any .Net mocking framework. 
Rhino Mocks "Classic", Rhino Mocks AAA, and Moq are supported out of the box

* Machine level overrides of default configuration

* Profile overrides of default configuration -- i.e. "Remote" vs "Local" vs
"Stubbed" dependency profiles

* Command line utility for troubleshooting runtime configuration issuesDiagnostic
querying of the Container configuration (Linq to StructureMap, sort of)"Assert"
configuration is validEnvironment Test SupportCustom Debugger

  * Diagnostic querying of the Container configuration (Linq to StructureMap, sort of)
  * "Assert" configuration is valid
  * Environment Test Support
  * Custom Debugger

* Uses Reflection.Emit namespace classes to create IL code on the fly for
performance superior to Reflection based approaches
