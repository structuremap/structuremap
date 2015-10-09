<!--Title:Replace or Clear Out Previous Registrations-->

Several members of the StructureMap team were also very active in a now semi-defunct web framework called [FubuMVC](http://github.com/darthfubumvc/fubumvc)
that was built with quite a bit of extensibility in mind. One of the extensibility mechanisms that was successful in FubuMVC was the ability for applications or addon libraries to swap out the default services in the main StructureMap application container.

The approach we took for this extensibility was what I flippantly call the "Mongolian BBQ" architecture. The framework should take the application specific registrations, the framework defaults, and all the discovered addons and figure out how to order the registrations to enforce the following levels of registration precedence:

1. The application specific registrations should always win out
1. Package or extension specific overrides
1. Default framework service registrations

To make this kind of modular and adaptive registration work, FubuMVC introduced a couple concepts that we've now pulled back into StructureMap:

1. The <[linkto:registration/fallback-services]> introduced in the previous topic where you can make a registration that effectively tells StructureMap to "use this registration if nobody else tells you something differently". 
1. The new `Registry.For().ClearAll()` mechanism shown in this topic that tells StructureMap to "disregard what everyone else said to use".

Typically in FubuMVC, we would use _fallback service_ registrations for most of the framework defaults and occasionally use the `ClearAll()` type mechanics down the line as an analogue to the CSS `!important` keyword to make a particular registration take precedence in the face of multiple registrations.

In usage, let's say that our application needs some type of `IWidget` service to run. For an important client, they want to deploy our system with
a special version, so we will create a new StructureMap `Registry` to apply their specific registrations using the `ClearAll()` mechanism to
insure that the important client gets their way:

<[sample:ImportantClientWidget]>

In usage, the `ClearAll()` stomps all over the default registration before adding their own:

<[sample:clear_all_in_action]>

If you were to check the <[linkto:diagnostics/whatdoihave]> view for `IWidget`, you would see only the `ImportantClientWidget`:

<pre style="overflow:scroll;word-break:normal;word-wrap:normal">
==============================================================================================================================================
PluginType     Namespace                           Lifecycle     Description                                                         Name     
----------------------------------------------------------------------------------------------------------------------------------------------
IWidget        StructureMap.Testing.Acceptance     Transient     StructureMap.Testing.Acceptance.clear_all+ImportantClientWidget     (Default)
==============================================================================================================================================
</pre>


