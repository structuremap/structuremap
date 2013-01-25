using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.Pipeline;
using StructureMap.TypeRules;
using System.Linq;

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

        private readonly Type _TPluggedType;
        private readonly SetterPropertyCollection _setters;
        private string _concreteKey;

        #region constructors

        public Plugin(Type TPluggedType, string concreteKey)
            : this(TPluggedType)
        {
            _concreteKey = concreteKey;
        }

        public Plugin(Type TPluggedType)
        {
            var att =
                Attribute.GetCustomAttribute(TPluggedType, typeof (PluggableAttribute), false) as PluggableAttribute;
            _concreteKey = att == null ? TPluggedType.AssemblyQualifiedName : att.ConcreteKey;

            _TPluggedType = TPluggedType;
            _setters = new SetterPropertyCollection(this);
            _constructor = new Constructor(TPluggedType);
        }

        #endregion

        /// <summary>
        /// The ConcreteKey that identifies the Plugin within a PluginFamily
        /// </summary>
        public string ConcreteKey { get { return _concreteKey; } set { _concreteKey = value; } }


        /// <summary>
        /// The concrete CLR Type represented by the Plugin
        /// </summary>
        public Type TPluggedType { get { return _TPluggedType; } }

        /// <summary>
        /// Property's that will be filled by setter injection
        /// </summary>
        public SetterPropertyCollection Setters { get { return _setters; } }

        public bool CanBeAutoFilled { get { return _constructor.CanBeAutoFilled() && _setters.CanBeAutoFilled(); } }

        public override string ToString()
        {
            return ("Plugin:  " + _concreteKey).PadRight(40) + TPluggedType.AssemblyQualifiedName;
        }


        public Instance CreateImplicitInstance()
        {
            return new ConfiguredInstance(TPluggedType).WithName(ConcreteKey);
        }

        public string FindArgumentNameForType<T>()
        {
            return FindArgumentNameForType(typeof (T), CannotFindProperty.ThrowException);
        }

        public string FindArgumentNameForEnumerableOf(Type type)
        {
            var enumerableTypes = EnumerableInstance.OpenEnumerableTypes.Select(x => x.MakeGenericType(type)).Union(new []{type.MakeArrayType()});
            return enumerableTypes.Select(t =>
            {
                return _constructor.FindFirstConstructorArgumentOfType(t) ??
                       _setters.FindFirstWriteablePropertyOfType(t);
            }).FirstOrDefault(x => x != null);
        }

        public string FindArgumentNameForType(Type type, CannotFindProperty cannotFind)
        {
            string returnValue =
                _constructor.FindFirstConstructorArgumentOfType(type) ??
                _setters.FindFirstWriteablePropertyOfType(type);

            if (returnValue == null && cannotFind == CannotFindProperty.ThrowException)
            {
                throw new StructureMapException(302, type.FullName, _TPluggedType.FullName);
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
            return Constructor.HasConstructors(_TPluggedType);
        }

        public bool HasOptionalSetters()
        {
            return _setters.OptionalCount > 0;
        }

        public Plugin CreateTemplatedClone(Type[] types)
        {
            Type templatedType = _TPluggedType.IsGenericType ? _TPluggedType.MakeGenericType(types) : _TPluggedType;

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
            return !_TPluggedType.IsOpenGeneric();
        }

        public Type FindArgumentType(string argumentName)
        {
            return _constructor.FindArgumentType(argumentName) ?? _setters.FindArgumentType(argumentName);
        }
    }
}