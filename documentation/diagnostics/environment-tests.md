<!--Title: Environment Tests-->
<!--Url: environment-tests-->

Years ago I worked with a legacy system that was particularly fragile in its deployment. While my team at the time and I made some serious improvements in the reliability of the automated deployment, the best thing we did was to add a set of _environment tests_ to the deployment that verified that basic elements of the system were working like:

* Could our code access the configured database?
* Was a certain COM object registered on the server? (I hated COM then and the years haven't changed my mind)
* Could we connect via remoting to another deployed application?

The deployments still frequently failed, but we were able to spot **and diagnose** the underlying problems much faster with our new environment tests than we could before by trying to run and debug the not-quite-valid application.

One of the mechanisms we used for these environment tests was StructureMap's ability to mark methods on configured types as environment tests with the `[ValidationMethod]` attribute as shown below:

<[sample:validation-method-usage]>

Used in conjunction with <[linkto:diagnostics/validating-container-configuration;title=StructureMap's ability to validate a container]>, you can use this technique to quickly support environment tests embedded into your system code.


