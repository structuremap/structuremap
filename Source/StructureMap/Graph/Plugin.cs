using System;
using System.Reflection;
using StructureMap.Emitting;
using StructureMap.Pipeline;

namespace StructureMap.Graph
{
    /// <summary>
    /// Represents a concrete class that can be built by StructureMap as an instance of the parent 
    /// PluginFamily’s PluginType. The properties of a Plugin are the CLR Type of the concrete class, 
    /// and the human-friendly concrete key that StructureMap will use to identify the Type.
    /// </summary>
    public class Plugin : TypeRules
    {
        public static readonly string DEFAULT = "DEFAULT";

        private string _concreteKey;
        private readonly Type _pluggedType;
        private readonly SetterPropertyCollection _setters;
        private readonly Constructor _constructor;

        #region constructors

        public Plugin(Type pluggedType, string concreteKey) : this(pluggedType)
        {
            if (concreteKey == string.Empty)
            {
                // TODO:  Move into PluginFamily and get the exception logged somewhere
                throw new StructureMapException(112, pluggedType.FullName);
            }

            _concreteKey = concreteKey;
        }

        public Plugin(Type pluggedType)
        {
            PluggableAttribute att = PluggableAttribute.InstanceOf(pluggedType);
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
            get
            {
                return _setters;
            }
        }

        public bool CanBeAutoFilled
        {
            get
            {
                return _constructor.CanBeAutoFilled() && _setters.CanBeAutoFilled();
            }
        }

        public override string ToString()
        {
            return ("Plugin:  " + _concreteKey).PadRight(40) + PluggedType.AssemblyQualifiedName;
        }


        public Instance CreateImplicitInstance()
        {
            return new ConfiguredInstance(PluggedType).WithConcreteKey(ConcreteKey).WithName(ConcreteKey);
        }

        public string FindFirstConstructorArgumentOfType<T>()
        {
            string returnValue = 
                _constructor.FindFirstConstructorArgumentOfType<T>() ??
                _setters.FindFirstConstructorArgumentOfType<T>();

            if (returnValue == null)
            {
                throw new StructureMapException(302, typeof(T).FullName, _pluggedType.FullName);    
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
    }

}