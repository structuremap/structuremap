<!--Title:Custom Instance Types-->

Most users will never need to implement a custom `Instance` type, but it does come up here and there. The easiest way is probably to
subclass `LambdaInstance` as shown below:

<[sample:FuncInstance]>

The other common customization is as a <[linkto:generics;title=generic builder template]>

If neither of these approaches will easily work for your custom `Instance`, you may want to contact the StructureMap team
via the gitter link above and we can help walk you through a custom subclass of `Instance` -- or more likely, talk you out
of it somehow.

## Missing Name Instance Template

New to StructureMap 4.0 is the `Instance.ToNamedClone(name)` method that is used behind the scenes by the <[linkto:the-container/handling-missing-named-instances]> feature. 

<[sample:ToNamedClose]>

Custom Instance types can override this method to provide a brand new Instance for a name. This functionality may be useful for multi-tenancy situations.