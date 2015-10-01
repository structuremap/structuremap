<!--Title:Type Scanning Diagnostics-->

<div class="alert alert-info" role="alert"><b><big>This functionality was introduced in StructureMap 4.0.</b></big></div>

Type scanning and conventional auto-registration is a very powerful feature in StructureMap, but it has been frequently troublesome to users when things go wrong. To try to alleviate problems, StructureMap 4.0 introduces some new functionality for detecting and diagnosing problems with type scanning, mostly related to Assembly's being missing.

## Assert for Assembly Loading Failures

At its root, most type scanning and auto-registration schemes in .Net frameworks rely on the <a href="https://msdn.microsoft.com/en-us/library/system.reflection.assembly.getexportedtypes%28v=vs.110%29.aspx">Assembly.GetExportedTypes()</a> method. Unfortunately, that method can be brittle and fail whenever any dependency of that Assembly cannot be loaded into the current process, even if your application has no need for that dependency. In StructureMap 3.* and before, that loading exception was essentially swallowed and lost. In StructureMap 4 and above, you can use this method to assert the presence of any assembly load exceptions during type scanning:

<[sample:assert-no-type-scanning-failures]>

The method above will throw an exception listing all the Assembly's that failed during the call to `GetExportedTypes()` only if there were any failures. Use this method during your application bootstrapping if you want it to fail fast with any type scanning problems.


## What did StructureMap scan?

Confusion of type scanning has been a constant problem with StructureMap usage over the years -- especially if users are trying to dynamically load assemblies from the file system for extensibility. In order to see into what StructureMap has done with type scanning, 4.0 introduces the `Container.WhatDidIScan()` method.

Let's say that you have a `Container` that is set up with at least two different scanning operations like this sample from the StructureMap unit tests:

<[sample:whatdidiscan]>

The resulting textual report is shown below:

_Sorry for the formatting and color of the text, but the markdown engine does **not** like the textual report_
<[sample:whatdidiscan-result]>

The textual report will show:

1. All the scanning operations (calls to `Registry.Scan()`) with a descriptive name, either one supplied by you or the `Registry` type and an order number.
1. All the assemblies that were part of the scanning operation including the assembly name, version, and a warning if `Assembly.GetExportedTypes()` failed on that assembly.
1. All the configured scanning conventions inside of the scanning operation

`WhatDidIScan()` does not at this time show any type filters or exclusions that may be part of the assembly scanner. 

See also: <[linkto:registration/auto-registration-and-conventions]>

