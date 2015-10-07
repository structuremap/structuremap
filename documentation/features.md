<!--Title: Features-->
<!--Url: features-->


<div class="alert alert-info" role="alert"><strong>Note!</strong> The StructureMap team will accept pull requests if there's something missing from this list that you'd like to add.
</div>


* Programmatic Configuration
* Conventional "Auto" Registration
* Modular configuration
* Constructor and Setter Injection
* Configurable constructor selection
* Customizable setter injection policies
* Auto wiring of dependencies
* Inline dependency configuration
* Service Location
* Strategies for registering and utilizing open generic types
* Lifecycle management and custom lifecycles
* ASP.Net centric lifecycles with [StructureMap.Web](https://www.nuget.org/packages/structuremap.web/)
* Interception and Configurable Decorators
* Lazy<T>, Func<T>, and Func<string, T> dependencies
* Auto resolution of enumerable types
* Auto resolution of concrete types
* "Profile's" for multi-tenancy or optional modes
* Diagnostics
* Auto resolution conventions
* "Auto Mocking*
* Simple Auto Factory support
* Nested and Child Containers


## Things StructureMap does not have

The following things are features that are not uncommon in other IoC tools or were present in previous versions of StructureMap.


* Xml configuration was removed for the 3.0 release. 
* Comprehensive attribute based configuration was also removed for the 3.0 release, but there is some very rudimentary support for attibute based
  configuration
* Complex Auto Factory support (this feature may be part of the StructureMap 4.0 release)

<div class="alert alert-info" role="alert"><strong>Note!</strong> The StructureMap team strongly feels that Xml configuration is both undesirable and unnecessary. Please feel free to use the <a href="https://groups.google.com/forum/#!forum/structuremap-users">Google user group</a> to ask for advice on how to move away from Xml configuration in existing applications.
</div>

