using System;
using System.Reflection;

namespace StructureMap.Graph
{
    public interface IPluginArgumentVisitor
    {
        void Primitive(string name);
        void Child(string name, Type childType);
        void ChildArray(string name, Type childType);
    }



    /// <summary>
    /// Represents a concrete class that can be built by StructureMap as an instance of the parent 
    /// PluginFamily’s PluginType. The properties of a Plugin are the CLR Type of the concrete class, 
    /// and the human-friendly concrete key that StructureMap will use to identify the Type.
    /// </summary>
    public class Plugin
    {
        #region static


        /// <summary>
        /// Determines if the PluggedType is a valid Plugin into the
        /// PluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="pluggedType"></param>
        /// <returns></returns>
        public static bool IsExplicitlyMarkedAsPlugin(Type pluginType, Type pluggedType)
        {
            bool returnValue = false;

            bool markedAsPlugin = PluggableAttribute.MarkedAsPluggable(pluggedType);
            if (markedAsPlugin)
            {
                returnValue = CanBeCast(pluginType, pluggedType);
            }

            return returnValue;
        }


        /// <summary>
        /// Determines if the pluggedType can be upcast to the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="pluggedType"></param>
        /// <returns></returns>
        public static bool CanBeCast(Type pluginType, Type pluggedType)
        {
            if (pluggedType.IsInterface || pluggedType.IsAbstract)
            {
                return false;
            }

            if (GenericsPluginGraph.CanBeCast(pluginType, pluggedType))
            {
                return true;
            }

            ConstructorInfo constructor = GetGreediestConstructor(pluggedType);
            if (constructor == null)
            {
                return false;
            }

            return pluginType.IsAssignableFrom(pluggedType);
        }


        public static ConstructorInfo GetGreediestConstructor(Type pluggedType)
        {
            ConstructorInfo returnValue = null;

            foreach (ConstructorInfo constructor in pluggedType.GetConstructors())
            {
                if (returnValue == null)
                {
                    returnValue = constructor;
                }
                else if (constructor.GetParameters().Length > returnValue.GetParameters().Length)
                {
                    returnValue = constructor;
                }
            }

            return returnValue;
        }

        #endregion

        private string _concreteKey;
        private Type _pluggedType;
        private SetterPropertyCollection _setters;

        #region constructors

        /// <summary>
        /// Creates an Explicit Plugin for the pluggedType with the entered
        /// concreteKey
        /// </summary>
        /// <param name="pluggedType"></param>
        /// <param name="concreteKey"></param>
        public Plugin(Type pluggedType, string concreteKey) : base()
        {
            if (concreteKey == string.Empty)
            {
                throw new StructureMapException(112, pluggedType.FullName);
            }

            _pluggedType = pluggedType;
            _concreteKey = concreteKey;
            _setters = new SetterPropertyCollection(this);
        }

        public Plugin(Type pluggedType)
        {
            PluggableAttribute att = PluggableAttribute.InstanceOf(pluggedType);
            _concreteKey = att == null ? pluggedType.AssemblyQualifiedName : att.ConcreteKey;

            _pluggedType = pluggedType;
            _setters = new SetterPropertyCollection(this);
            
        }

        #endregion


        /// <summary>
        /// The ConcreteKey that identifies the Plugin within a PluginFamily
        /// </summary>
        public string ConcreteKey
        {
            get { return _concreteKey; }
            set { _concreteKey = value; }
        }


        /// <summary>
        /// The concrete CLR Type represented by the Plugin
        /// </summary>
        public Type PluggedType
        {
            get { return _pluggedType; }
        }

        /// <summary>
        /// Finds any methods on the PluggedType marked with the [ValidationMethod]
        /// attributes
        /// </summary>
        public MethodInfo[] ValidationMethods
        {
            get { return ValidationMethodAttribute.GetValidationMethods(_pluggedType); }
        }

        /// <summary>
        /// Property's that will be filled by setter injection
        /// </summary>
        public SetterPropertyCollection Setters
        {
            get
            {
                if (_setters == null)
                {
                    _setters = new SetterPropertyCollection(this);
                }

                return _setters;
            }
        }


        /// <summary>
        /// Determines if the concrete class can be autofilled.
        /// </summary>
        public bool CanBeAutoFilled
        {
            get
            {
                bool returnValue = true;

                ConstructorInfo ctor = GetConstructor();
                foreach (ParameterInfo parameter in ctor.GetParameters())
                {
                    returnValue = returnValue && TypeCanBeAutoFilled(parameter.ParameterType);
                }

                foreach (SetterProperty setter in Setters)
                {
                    Type propertyType = setter.Property.PropertyType;
                    returnValue = returnValue && TypeCanBeAutoFilled(propertyType);
                }

                return returnValue;
            }
        }


        /// <summary>
        /// Returns the System.Reflection.ConstructorInfo for the PluggedType.  Uses either
        /// the "greediest" constructor with the most arguments or the constructor function
        /// marked with the [DefaultConstructor]
        /// </summary>
        /// <returns></returns>
        public ConstructorInfo GetConstructor()
        {
            ConstructorInfo returnValue = DefaultConstructorAttribute.GetConstructor(_pluggedType);

            // if no constructor is marked as the "ContainerConstructor", find the greediest constructor
            if (returnValue == null)
            {
                returnValue = GetGreediestConstructor(_pluggedType);
            }

            if (returnValue == null)
            {
                throw new StructureMapException(180, _pluggedType.Name);
            }

            return returnValue;
        }


        /// <summary>
        /// Boolean flag denoting the presence of any constructor arguments
        /// </summary>
        /// <returns></returns>
        public bool HasConstructorArguments()
        {
            return (GetConstructor().GetParameters().Length > 0);
        }

        public override string ToString()
        {
            return ("Plugin:  " + _concreteKey).PadRight(40) + PluggedType.AssemblyQualifiedName;
        }

        /// <summary>
        /// Creates an InstanceMemento for a PluggedType that requires no
        /// configuration.  I.e. a CLR Type that has no constructor functions or 
        /// is marked as "[AutoFilled]"
        /// </summary>
        /// <returns></returns>
        public InstanceMemento CreateImplicitMemento()
        {
            InstanceMemento returnValue = null;

            if (CanBeAutoFilled)
            {
                MemoryInstanceMemento memento = new MemoryInstanceMemento(ConcreteKey, ConcreteKey);
                returnValue = memento;
            }

            return returnValue;
        }

        public static bool TypeCanBeAutoFilled(Type parameterType)
        {
            bool cannotBeFilled = false;

            cannotBeFilled = cannotBeFilled || parameterType.IsValueType;
            cannotBeFilled = cannotBeFilled || parameterType.IsArray;
            cannotBeFilled = cannotBeFilled || parameterType.Equals(typeof (string));

            return !cannotBeFilled;
        }

       
        public static bool TypeIsPrimitive(Type parameterType)
        {
            return parameterType.IsValueType || parameterType.Equals(typeof (string));
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            Plugin plugin = obj as Plugin;
            if (plugin == null) return false;
            return Equals(_pluggedType, plugin._pluggedType) && Equals(_concreteKey, plugin._concreteKey);
        }

        public override int GetHashCode()
        {
            return
                (_pluggedType != null ? _pluggedType.GetHashCode() : 0) +
                29*(_concreteKey != null ? _concreteKey.GetHashCode() : 0);
        }

        public string FindFirstConstructorArgumentOfType<T>()
        {
            ConstructorInfo ctor = GetConstructor();
            foreach (ParameterInfo info in ctor.GetParameters())
            {
                if (info.ParameterType.Equals(typeof (T)))
                {
                    return info.Name;
                }
            }

            throw new StructureMapException(302, typeof (T).FullName, _pluggedType.FullName);
        }

        public void VisitArguments(IPluginArgumentVisitor visitor)
        {
            VisitConstructorArguments(visitor);
            VisitSetterArguments(visitor);
        }

        private void VisitSetterArguments(IPluginArgumentVisitor visitor)
        {
            foreach (SetterProperty setter in _setters)
            {
                visitMember(setter.Property.PropertyType, setter.Property.Name, visitor);
            }
        }

        private void VisitConstructorArguments(IPluginArgumentVisitor visitor)
        {
            foreach (ParameterInfo parameter in GetConstructor().GetParameters())
            {
                visitMember(parameter.ParameterType, parameter.Name, visitor);
            }
        }

        private static void visitMember(Type type, string name, IPluginArgumentVisitor visitor)
        {
            if (TypeIsPrimitive(type))
            {
                visitor.Primitive(name);
            }
            else if (type.IsArray)
            {
                visitor.ChildArray(name, type.GetElementType());
            }
            else
            {
                visitor.Child(name, type);
            }
        }

        public void MergeSetters(Plugin plugin)
        {
            foreach (SetterProperty setter in plugin.Setters)
            {
                if (!_setters.Contains(setter.Name))
                {
                    _setters.Add(setter.Name);
                }
            }
        }

        // TODO: Move to Generics.  This code is insanely stupid.  Just make the templated type and do
        // IsAssignableFrom.  Duh!
        public bool CanBePluggedIntoGenericType(Type pluginType, params Type[] templateTypes)
        {
            bool isValid = true;

            Type interfaceType = PluggedType.GetInterface(pluginType.Name);
            if (interfaceType == null)
            {
                interfaceType = PluggedType.BaseType;
            }

            Type[] pluginArgs = pluginType.GetGenericArguments();
            Type[] pluggableArgs = interfaceType.GetGenericArguments();

            if (templateTypes.Length != pluginArgs.Length &&
                pluginArgs.Length != pluggableArgs.Length)
            {
                return false;
            }

            for (int i = 0; i < templateTypes.Length; i++)
            {
                isValid &= templateTypes[i] == pluggableArgs[i] ||
                           pluginArgs[i].IsGenericParameter &&
                           pluggableArgs[i].IsGenericParameter;
            }
            return isValid;
        }
    }
}