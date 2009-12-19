using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    /// <summary>
    /// Represents a concrete class that can be built by StructureMap as an instance of the parent 
    /// PluginFamily’s PluginType. The properties of a Plugin are the CLR Type of the concrete class, 
    /// and the human-friendly concrete key that StructureMap will use to identify the Type.
    /// </summary>
    public class Plugin
    {
        public static readonly string DEFAULT = "DEFAULT";
        private readonly Constructor _constructor;

        private readonly Type _pluggedType;
        private readonly SetterPropertyCollection _setters;
        private string _concreteKey;

        #region constructors

        public Plugin(Type pluggedType, string concreteKey) : this(pluggedType)
        {
            _concreteKey = concreteKey;
        }

        public Plugin(Type pluggedType)
        {
            PluggableAttribute att = 
                Attribute.GetCustomAttribute(pluggedType, typeof (PluggableAttribute), false) as PluggableAttribute;
            _concreteKey = att == null ? pluggedType.AssemblyQualifiedName : att.ConcreteKey;

            _pluggedType = pluggedType;
            _setters = new SetterPropertyCollection(this);
            _constructor = new Constructor(pluggedType);
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
        /// Property's that will be filled by setter injection
        /// </summary>
        public SetterPropertyCollection Setters
        {
            get { return _setters; }
        }

        public bool CanBeAutoFilled
        {
            get { return _constructor.CanBeAutoFilled() && _setters.CanBeAutoFilled(); }
        }

        public override string ToString()
        {
            return ("Plugin:  " + _concreteKey).PadRight(40) + PluggedType.AssemblyQualifiedName;
        }


        public Instance CreateImplicitInstance()
        {
            return new ConfiguredInstance(PluggedType).WithName(ConcreteKey);
        }

        public string FindArgumentNameForType<T>()
        {
            return FindArgumentNameForType(typeof (T));
        }

        public string FindArgumentNameForType(Type type)
        {
            string returnValue =
                _constructor.FindFirstConstructorArgumentOfType(type) ??
                _setters.FindFirstConstructorArgumentOfType(type);

            if (returnValue == null)
            {
                throw new StructureMapException(302, type.FullName, _pluggedType.FullName);
            }

            return returnValue;
        }

        public void VisitArguments(IArgumentVisitor visitor)
        {
            _constructor.Visit(visitor);
            _setters.Visit(visitor);
        }

        public void MergeSetters(Plugin plugin)
        {
            Setters.Merge(plugin.Setters);
        }

        public ConstructorInfo GetConstructor()
        {
            return _constructor.Ctor;
        }

        public void VisitConstructor(IArgumentVisitor arguments)
        {
            _constructor.Visit(arguments);
        }

        public void VisitSetters(IArgumentVisitor arguments)
        {
            _setters.Visit(arguments);
        }

        public bool IsValid()
        {
            return _constructor.IsValid();
        }

        public bool CanBeCreated()
        {
            return Constructor.HasConstructors(_pluggedType);
        }

        public bool HasOptionalSetters()
        {
            return _setters.OptionalCount > 0;
        }

        public Plugin CreateTemplatedClone(Type[] types)
        {
            Type templatedType = _pluggedType.IsGenericType ? _pluggedType.MakeGenericType(types) : _pluggedType;

            var templatedPlugin = new Plugin(templatedType, ConcreteKey);

            foreach (SetterProperty setter in Setters)
            {
                templatedPlugin.Setters.MarkSetterAsMandatory(setter.Name);
            }

            return templatedPlugin;
        }

        public void UseConstructor(Expression expression)
        {
            _constructor.UseConstructor(expression);
        }

        public void UseSetterRule(Predicate<PropertyInfo> rule)
        {
            _setters.UseSetterRule(rule);
        }

        public bool IsNotOpenGeneric()
        {
            return !_pluggedType.IsGeneric();
        }
    }
}