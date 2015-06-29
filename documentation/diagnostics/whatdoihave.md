<!--Title: WhatDoIHave()-->
<!--Url: whatdoihave-->


The `IContainer.WhatDoIHave()` method can give you a quick textual report of the current configuration of a running `Container`:

<[sample:whatdoihave-simple]>

If you're familiar with `WhatDoIHave()` from earlier versions of StructureMap, the usage has been enhanced for 3.0 to allow you
to filter the results for easier usage. The format was also tweaked extensively to (hopefully) improve the usability of this feature.

Enough talk, say you have a `Container` with this configuration:

<[sample:what_do_i_have_container]>

If you were to run the code below against this `Container`:

<[sample:whatdoihave_everything]>

you would get the output shown in <a href="https://gist.githubusercontent.com/jeremydmiller/907e1deb2553a5ca5b18/raw/8b57456f36ccc5043e61d09e72b1ab5fcea94718/gistfile1.txt" target="_new">this gist</a>.


If you're curious, all the raw code for this example is in [here](https://github.com/structuremap/structuremap/blob/master/src/StructureMap.Testing/WhatDoIHave_Smoke_Tester.cs).

## Filtering WhatDoIHave()

Filtering the `WhatDoIHave()` results can be done in these ways:

<[sample:whatdoihave-filtering]>
