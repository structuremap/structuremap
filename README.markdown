Welcome to StructureMap, the oldest [Inversion of Control][1] container for .Net. 

Getting StructureMap
--------------------

[![Join the chat at https://gitter.im/structuremap/structuremap](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/structuremap/structuremap?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

StructureMap is available via NuGet:

```PowerShell
Install-Package StructureMap
```

or:

```PowerShell
ripple install StructureMap -p [your project name]
```

If you want to fix a bug or just want to tinker with an idea,
we love receiving pull requests!

Building the Source
-------------------

1. Clone the repository: `git clone git://github.com/structuremap/structuremap.git`
1. Make sure that you have got [Ruby][2] (>= 1.9.3) installed.
1. If you are installing Ruby for the first time, run the following in command prompt at the root of the solution:
    1. `gem install bundler`
    2. `bundle install`
1. In the root of the solution, run `rake` in the command prompt.
1. Open `StructureMap.sln` in VS2012 (`rake sln` from the root as well).

A copy of the [StructureMap website and documentation][3] is
in the `StructureMap.Docs` folder.  To run the documentation
website, run `rake fubudocs:run` or `fubudocs run -o` in the
command prompt.

Please post any questions or bugs to the
[StructureMap Users mailing list][4].

The latest code and documentation is available on the
[main site][5] and on [Fubuworld][6].

Thanks for trying StructureMap.

[1]: http://docs.structuremap.net/InversionOfControl.htm
[2]: http://www.ruby-lang.org/en/downloads/
[3]: http://docs.structuremap.net/
[4]: http://groups.google.com/group/structuremap-users
[5]: http://structuremap.net/
[6]: http://fubuworld.com/structuremap