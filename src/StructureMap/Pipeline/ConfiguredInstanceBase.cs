using System;
using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public interface BasicInstance
    {
        Type TPluggedType { get; }


        Dictionary<string, string> Properties { get; }
        Dictionary<string, Instance> Children { get; }
        Dictionary<string, Instance[]> Arrays { get; }
    }


    //public abstract class ConfiguredInstanceBase<T> : Instance, IConfiguredInstance, IStructuredInstance, BasicInstance
    //{
    //    protected Dictionary<string, Instance[]> _arrays = new Dictionary<string, Instance[]>();
    //    protected Dictionary<string, Instance> _children = new Dictionary<string, Instance>();
    //    protected Type _TPluggedType;
    //    protected Dictionary<string, string> _properties = new Dictionary<string, string>();

    //    protected ConfiguredInstanceBase(InstanceMemento memento, PluginGraph graph, Type pluginType)
    //    {
    //        read(memento, graph, pluginType);
    //    }

    //    protected ConfiguredInstanceBase(Type PluggedType, string name)
    //    {
    //        _TPluggedType = PluggedType;
    //        Name = name;
    //    }

    //    protected ConfiguredInstanceBase(Type PluggedType) : this(PluggedType, Guid.NewGuid().ToString())
    //    {
    //        if (PluggedType != null)
    //            if (PluggedType.IsAbstract || PluggedType.IsInterface)
    //            {
    //                throw new StructureMapException(25);
    //            }
    //    }

    //    #region Copyable Members

    //    Type BasicInstance.PluggedType
    //    {
    //        get { return _TPluggedType; }
    //    }

    //    Dictionary<string, string> BasicInstance.Properties
    //    {
    //        get { return _properties; }
    //    }

    //    Dictionary<string, Instance> BasicInstance.Children
    //    {
    //        get { return _children; }
    //    }

    //    Dictionary<string, Instance[]> BasicInstance.Arrays
    //    {
    //        get { return _arrays; }
    //    }

    //    #endregion

    //    //#region IConfiguredInstance Members

    //    //Type IConfiguredInstance.PluggedType
    //    //{
    //    //    get { return _TPluggedType; }
    //    //}

    //    //Instance[] IConfiguredInstance.GetChildrenArray(string propertyName)
    //    //{
    //    //    return _arrays.ContainsKey(propertyName) ? _arrays[propertyName] : null;
    //    //}

    //    //string IConfiguredInstance.GetProperty(string propertyName)
    //    //{
    //    //    if (!_properties.ContainsKey(propertyName))
    //    //    {
    //    //        throw new StructureMapException(205, propertyName, Name);
    //    //    }

    //    //    return _properties[propertyName];
    //    //}

    //    //object IConfiguredInstance.Get(string propertyName, Type pluginType, BuildSession buildSession)
    //    //{
    //    //    return getChild(propertyName, pluginType, buildSession);
    //    //}

    //    //object IConfiguredInstance.Build(Type pluginType, BuildSession session, InstanceBuilder builder)
    //    //{
    //    //    if (builder == null)
    //    //    {
    //    //        throw new StructureMapException(
    //    //            201, _TPluggedType.FullName, Name, pluginType);
    //    //    }


    //    //    try
    //    //    {
    //    //        return builder.BuildInstance(this, session);
    //    //    }
    //    //    catch (StructureMapException)
    //    //    {
    //    //        throw;
    //    //    }
    //    //    catch (InvalidCastException ex)
    //    //    {
    //    //        throw new StructureMapException(206, ex, Name);
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        throw new StructureMapException(207, ex, Name, pluginType.FullName);
    //    //    }
    //    //}

    //    //bool IConfiguredInstance.HasProperty(string propertyName, BuildSession session)
    //    //{
    //    //    return _properties.ContainsKey(propertyName) || _children.ContainsKey(propertyName) ||
    //    //           _arrays.ContainsKey(propertyName);
    //    //}

    //    //void IConfiguredInstance.SetProperty(string propertyName, string propertyValue)
    //    //{
    //    //    setProperty(propertyName, propertyValue);
    //    //}

    //    //void IConfiguredInstance.SetChild(string name, Instance instance)
    //    //{
    //    //    setChild(name, instance);
    //    //}

    //    //void IConfiguredInstance.SetChildArray(string name, Type type, Instance[] children)
    //    //{
    //    //    setChildArray(name, children);
    //    //}

    //    //#endregion

    //    #region IStructuredInstance Members

    //    Instance IStructuredInstance.GetChild(string name)
    //    {
    //        return _children[name];
    //    }

    //    Instance[] IStructuredInstance.GetChildArray(string name)
    //    {
    //        return _arrays[name];
    //    }

    //    void IStructuredInstance.RemoveKey(string name)
    //    {
    //        _properties.Remove(name);
    //    }

    //    #endregion

    //    protected override object build(Type pluginType, BuildSession session)
    //    {
    //        InstanceBuilder builder = PluginCache.FindBuilder(_TPluggedType);
    //        return ((IConfiguredInstance) this).Build(pluginType, session, builder);
    //    }

    //    protected virtual object getChild(string propertyName, Type pluginType, BuildSession buildSession)
    //    {
    //        Instance childInstance = _children.ContainsKey(propertyName)
    //                                     ? _children[propertyName]
    //                                     : new DefaultInstance();


    //        return childInstance.Build(pluginType, buildSession);
    //    }

    //    protected override bool canBePartOfPluginFamily(PluginFamily family)
    //    {
    //        return _TPluggedType.CanBeCastTo(family.PluginType) && Constructor.HasConstructors(_TPluggedType);
    //    }

    //    internal override bool Matches(Plugin plugin)
    //    {
    //        return plugin.PluggedType == _TPluggedType;
    //    }

    //    private void read(InstanceMemento memento, PluginGraph graph, Type pluginType)
    //    {
    //        PluginFamily family = graph.FindFamily(pluginType);
    //        Plugin plugin = memento.FindPlugin(family);

    //        _TPluggedType = plugin.PluggedType;

    //        var reader = new InstanceMementoPropertyReader(this, memento, graph, pluginType);
    //        plugin.VisitArguments(reader);
    //    }

    //    protected void setChild(string name, Instance instance)
    //    {
    //        if (instance == null) return;

    //        if (_children.ContainsKey(name))
    //        {
    //            _children[name] = instance;
    //        }
    //        else
    //        {
    //            _children.Add(name, instance);
    //        }


    //    }

    //    protected void setChildArray(string name, Instance[] array)
    //    {
    //        for (int i = 0; i < array.Length; i++)
    //        {
    //            if (array[i] == null)
    //            {
    //                throw new ApplicationException("There is a null value in the array of child Instances");
    //            }

    //        }

    //        _arrays.Add(name, array);
    //    }

    //    protected override Type getConcreteType(Type pluginType)
    //    {
    //        return _TPluggedType;
    //    }

    //    protected string findPropertyName<TPluginType>()
    //    {
    //        Type dependencyType = typeof(TPluginType);

    //        return findPropertyName(dependencyType);
    //    }

    //    protected string findPropertyName(Type dependencyType)
    //    {
    //        var plugin = new Plugin(_TPluggedType);
    //        string propertyName = plugin.FindArgumentNameForType(dependencyType);

    //        if (string.IsNullOrEmpty(propertyName))
    //        {
    //            throw new StructureMapException(305, dependencyType);
    //        }

    //        return propertyName;
    //    }

    //    protected static void validateTypeIsArray<TPluginType>()
    //    {
    //        validateTypeIsArray(typeof(TPluginType));
    //    }

    //    protected static void validateTypeIsArray(Type pluginType)
    //    {
    //        if (!pluginType.IsArray)
    //        {
    //            throw new StructureMapException(307);
    //        }
    //    }

    //    protected void setProperty(string propertyName, string propertyValue)
    //    {
    //        if (string.IsNullOrEmpty(propertyValue)) return;
    //        _properties[propertyName] = propertyValue;
    //    }

    //    protected void mergeIntoThis(BasicInstance instance)
    //    {
    //        _TPluggedType = instance.PluggedType;

    //        foreach (var pair in instance.Properties)
    //        {
    //            if (!_properties.ContainsKey(pair.Key))
    //            {
    //                _properties.Add(pair.Key, pair.Value);
    //            }
    //        }

    //        foreach (var pair in instance.Children)
    //        {
    //            if (!_children.ContainsKey(pair.Key))
    //            {
    //                _children.Add(pair.Key, pair.Value);
    //            }
    //        }

    //        _arrays = instance.Arrays;
    //    }
    //}
}