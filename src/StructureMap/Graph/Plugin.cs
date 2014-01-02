using System;
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
    [Obsolete("This is going away after the BuildPlan stuff is completely in")]
    public class Plugin
    {
        public static readonly string DEFAULT = "DEFAULT";
        private readonly Constructor _constructor;

        private readonly Type _pluggedType;
        private readonly SetterPropertyCollection _setters;

        #region constructors

        public Plugin(Type pluggedType)
        {
            _pluggedType = pluggedType;
            _setters = new SetterPropertyCollection(this);
            _constructor = new Constructor(pluggedType);
        }

        #endregion


        /// <summary>
        /// The concrete CLR Type represented by the Plugin
        /// </summary>
        public Type PluggedType { get { return _pluggedType; } }

        /// <summary>
        /// Property's that will be filled by setter injection
        /// </summary>
        public SetterPropertyCollection Setters { get { return _setters; } }

        public override string ToString()
        {
            return "Plugin:  " +  PluggedType.AssemblyQualifiedName;
        }

        public string FindArgumentNameForType<T>()
        {
            return FindArgumentNameForType(typeof (T), CannotFindProperty.ThrowException);
        }

        public string FindArgumentNameForType(Type type, CannotFindProperty cannotFind)
        {
            string returnValue =
                _constructor.FindFirstConstructorArgumentOfType(type) ??
                _setters.FindFirstWriteablePropertyOfType(type);

            if (returnValue == null && cannotFind == CannotFindProperty.ThrowException)
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

            var templatedPlugin = new Plugin(templatedType);

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

        public void UseSetterRule(Func<PropertyInfo, bool> rule)
        {
            _setters.UseSetterRule(rule);
        }

        public bool IsNotOpenGeneric()
        {
            return !_pluggedType.IsOpenGeneric();
        }

        public Type FindArgumentType(string argumentName)
        {
            return _constructor.FindArgumentType(argumentName) ?? _setters.FindArgumentType(argumentName);
        }
    }
}