using System;
using System.Collections.Generic;
using System.Text;

namespace StructureMap.Pipeline
{
    public partial class ConfiguredInstance
    {
        Type IConfiguredInstance.PluggedType
        {
            get { return _pluggedType; }
        }

        Instance[] IConfiguredInstance.GetChildrenArray(string propertyName)
        {
            return _arrays.ContainsKey(propertyName) ? _arrays[propertyName] : null;
        }

        string IConfiguredInstance.GetProperty(string propertyName)
        {
            if (!_properties.ContainsKey(propertyName))
            {
                throw new StructureMapException(205, propertyName, Name);
            }

            return _properties[propertyName];
        }

        object IConfiguredInstance.GetChild(string propertyName, Type pluginType, IBuildSession buildSession)
        {
            return getChild(propertyName, pluginType, buildSession);
        }

        string IConfiguredInstance.ConcreteKey
        {
            get { return _concreteKey; }
            set { _concreteKey = value; }
        }


        // Only open for testing
        object IConfiguredInstance.Build(Type pluginType, IBuildSession session, InstanceBuilder builder)
        {
            if (builder == null)
            {
                throw new StructureMapException(
                    201, _concreteKey, Name, pluginType);
            }


            try
            {
                return builder.BuildInstance(this, session);
            }
            catch (StructureMapException)
            {
                throw;
            }
            catch (InvalidCastException ex)
            {
                throw new StructureMapException(206, ex, Name);
            }
            catch (Exception ex)
            {
                throw new StructureMapException(207, ex, Name, pluginType.FullName);
            }
        }

    }
}
