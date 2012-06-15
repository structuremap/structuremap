---
title: Attribute Normalized Instances
layout: default
---

This style is a more terse configuration format that was added in version 1.0. 
My recommendaton is to always use attribute normalized                        
Xml configuration by marking the `<StructureMap>` node with the                 
MementoStyle="Attribute" attribute by default anytime you are using Xml         
configuration. 


Instance Root Node
=================================


An Instance is defined in Xml starting from a root node.  The actual name of    
the root node varies depending upon the context that an Instance is being       
configured.  For example, the AddInstance, DefaultInstance,        
PluginFamily/Instance, Interceptor, and Source nodes in Xml configuration are   
all Instance roots.  The rules for an Instance node are the same regardless     
of the context.  The root node optionally specifies the name of the        
instance and the concrete type of the instance.


For a class called ColorRule,


{% highlight csharp %}
[Pluggable("Color")]
public class ColorRule : Rule
{
    private string _Color;
    public string ID = Guid.NewGuid().ToString()
	 
    public ColorRule(string Color)
    {
        _Color = Color;
    }
					 
    public string Color
    {
				get { return _Color; }
    }
}
{% endhighlight %}


an Instance node might look like:


{% highlight xml %}
<Instance Key="Red" Type="Color" Color="Red" />
{% endhighlight %}


The "name" of an Instance is definied by the Key attribute.  The concrete    
type can be specified in one of two ways.  You can either use the "Type"    
attribute above to specify the aliased concrete type, or do it more explicitly  
by setting the PluggedType attribute to the assembly qualified name of the    
concrete type.


{% highlight xml %}
<Instance Key="Red" PluggedType="StructureMap.Testing.Widget.Color, StructureMap.Testing.Widget" Color="Red" />
{% endhighlight %}


Primitive Properties (Strings and basic value types)
=================================


Primitive constructor or setter arguments are defined by adding an attribute    
@propertyName="propertyValue" to the instance node.  A class with               
a string argument to the constructor,


{% highlight csharp %}
[Pluggable("Color", "Only for testing")]    public class ColorWidget : IWidget
{
    public ColorWidget(string Color)
    {
        _Color = Color;
    }
}
    
public class ColorWidget : IWidget
{
    public ColorWidget(string Color)
    {
        _Color = Color;
    }
}
{% endhighlight %}


would be configured like this in the Xml:


{% highlight xml %}
<Widget Type="Color" Key="Red" Color="Red" />
{% endhighlight %}


Long Strings
=================================


There is an optional mode to define a property value inside a CDATA tag for very
long strings like sql statements or Javascript templates. 


{% highlight xml %}
<Instance Type="Sql" Key="SomeQuery">
  <bigProp>
     <![CDATA[
       select * from table
       where
         somecolumn = 'something' or
         some_other_column = 'something else'
     ]]>
   </bigProp>
</Property>
{% endhighlight %}


Enumeration Properties
=================================


Enumeration arguments are defined the same way as primitive properties.  Use the
string names of the enumeration for the values.


{% highlight csharp %}
public enum BreedEnum
{
    Hereford,
    Angus,
    Longhorn
}
 
[Pluggable("Cow")]
public class Cow
{
    public BreedEnum Breed;
    public long Weight;
    public string Name;

    public Cow(long Weight, BreedEnum Breed, string Name)
    {
        this.Breed = Breed;
        this.Weight = Weight;
        this.Name = Name;
    }
}
{% endhighlight %}


{% highlight xml %}
<Instance Type="Cow" Key="Maggie" Breed="Angus" />
{% endhighlight %}


Dependency Properties
=================================


Child properties of non-primitive types are defined as embedded memento nodes. 
Child properties can be either defined inline or use a reference to a named
instance of the property type.  If a child property is omitted or defined with
no value, StructureMap will use the default instance of the property type. 
Simply add a child node to the main instance                         node with
the name of the constructor argument or setter property name.


{% highlight csharp %}
[PluginFamily, Pluggable("Default", "")]
public class GrandChild
{
    public GrandChild(bool RightHanded, int BirthYear)
    {
    }
}
 
[Pluggable("Leftie", "")]
public class LeftieGrandChild : GrandChild
{
    public LeftieGrandChild(int BirthYear) : base(false, BirthYear)
    {
    }
}
 
[PluginFamily, Pluggable("Default", "")]
public class Child
{
    public Child(string Name, GrandChild MyGrandChild)
    {
    }
}
{% endhighlight %}

**Inline Definition**

{% highlight xml %}
<StructureMap.Testing.Widget.Child Type="Default" Key="Marsha" Name="Marsha">
    <MyGrandChild Key="Tommy"/>
</StructureMap.Testing.Widget.Child>    
{% endhighlight %}


Non Primitive Array Property
=================================


If a property or constructor argument is an array of a non-primitive type,
create     a child node to the top level instance node with the name of the
property.      Simply add new InstanceMemento nodes with the name `<Child>`
under the property     nodes for each element of the array.  These `<Child>`
nodes are Attribute     Normalized InstanceMemento's and follow the same rules
expressed in this     document.


{% highlight csharp %}
[Pluggable("Compound")]
public class CompoundStrategy : IStrategy
{
    public CompoundStrategy(IStrategy[] innerStrategies)
    {
    }
}
{% endhighlight %}


{% highlight xml %}
<Instance Key="ArrayTest" Type="Compound">
    <innerStrategies>
        <!-- Referenced Instance -->
        <Child Key="Red" />
 
        <Child><!-- Default Instance --></Child>
 
        <!-- Inline Definition -->
        <Child Type="Random" seed="0.034"/>
    </innerStrategies>
</Instance>
{% endhighlight %}


Primitive Arrays
=================================


Primitive arrays like string[] or int[] can be defined in Xml.  For a class     
with arguments like:


{% highlight csharp %}
public ClassWithStringAndIntArray(int[] numbers, string[] strings)
{
    _numbers = numbers;
    _strings = strings;
}
{% endhighlight %}


The Xml configuration is:


{% highlight xml %}
<DefaultInstance PluginType="StructureMap.Testing.Configuration.ClassWithStringAndIntArray, StructureMap.Testing"
  PluggedType="StructureMap.Testing.Configuration.ClassWithStringAndIntArray, StructureMap.Testing">
    <numbers Values="1,2,3"/>
    <strings Values="1,2,3"/>
</DefaultInstance>
{% endhighlight %}


By default, the Values attribute is assumed to be a comma delimited list.     
The delimiter of the list can be optionally overriden by using the Delimiter    
attribute.


{% highlight xml %}
<DefaultInstance
	PluginType="StructureMap.Testing.Configuration.ClassWithStringAndIntArray, StructureMap.Testing"
	PluggedType="StructureMap.Testing.Configuration.ClassWithStringAndIntArray, StructureMap.Testing">
 
    <numbers Values="1;2;3" Delimiter=";" />
    <strings Values="1,2,3" />
</DefaultInstance>
{% endhighlight %}


Dictionaries and NameValueCollection
=================================


Any form of `IDictionary<Key, Value>` or a NameValueCollection can be configured
in     Xml by the following syntax.  Say you have a class that needs a
Dictionary     of properties:


{% highlight csharp %}
    public class ClassWithDictionary
    {
        private readonly IDictionary<string, string> _dictionary;
         
        public ClassWithDictionary(IDictionary<string, string> dictionary)
        {
            _dictionary = dictionary;
        }
         
        public IDictionary<string, string> Dictionary
        {
             get { return _dictionary; }
        }
    }
{% endhighlight %}


The "dictionary" argument to the constructor function could be defined as:


{% highlight xml %}
<DefaultInstance
		PluginType="StructureMap.Testing.Configuration.ClassWithDictionary, StructureMap.Testing"
		PluggedType="StructureMap.Testing.Configuration.ClassWithDictionary, StructureMap.Testing">
  <dictionary Key="color" Value="red"/>
    <Pair Key="state" Value="texas"/>
    <Pair Key="direction" Value="north"/>
  </dictionary>
</DefaultInstance>
{% endhighlight %}


Just create a new node for the IDictionary property called <[propertyName]>
under     the main instance node.  Then add a <Pair Key="key" Value="value"/>
node     for each name/value pair in the IDictionary.

