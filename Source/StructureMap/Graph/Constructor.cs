using System;
using System.Reflection;

namespace StructureMap.Graph
{
    public class Constructor : TypeRules
    {
        /// <summary>
        /// Returns the System.Reflection.ConstructorInfo for the PluggedType.  Uses either
        /// the "greediest" constructor with the most arguments or the constructor function
        /// marked with the [DefaultConstructor]
        /// </summary>
        /// <returns></returns>
        public static ConstructorInfo GetConstructor(Type pluggedType)
        {
            ConstructorInfo returnValue = DefaultConstructorAttribute.GetConstructor(pluggedType);

            // if no constructor is marked as the "ContainerConstructor", find the greediest constructor
            if (returnValue == null)
            {
                returnValue = GetGreediestConstructor(pluggedType);
            }

            if (returnValue == null)
            {
                throw new StructureMapException(180, pluggedType.Name);
            }

            return returnValue;
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


        private readonly Type _pluggedType;
        private ConstructorInfo _ctor;

        public Constructor(Type pluggedType)
        {
            _pluggedType = pluggedType;
            _ctor = GetConstructor(pluggedType);
        }

        public bool CanBeAutoFilled()
        {
            foreach (ParameterInfo parameter in _ctor.GetParameters())
            {
                if (!IsChild(parameter.ParameterType))
                {
                    return false;
                }
            }

            return true;
        }

        public string FindFirstConstructorArgumentOfType<T>()
        {
            foreach (ParameterInfo info in _ctor.GetParameters())
            {
                if (info.ParameterType.Equals(typeof(T)))
                {
                    return info.Name;
                }
            }

            return null;
        }


        public ConstructorInfo Ctor
        {
            get { return _ctor; }
        }

        public void Visit(IArgumentVisitor visitor)
        {
            foreach (ParameterInfo info in _ctor.GetParameters())
            {
                Type parameterType = info.ParameterType;

                visitParameter(info, parameterType, visitor);
            }
        }

        private void visitParameter(ParameterInfo info, Type parameterType, IArgumentVisitor visitor)
        {
            if (IsPrimitive(parameterType)) visitor.PrimitiveParameter(info);
            if (IsChild(parameterType)) visitor.ChildParameter(info);
            if (IsChildArray(parameterType)) visitor.ChildArrayParameter(info);
            if (IsEnum(parameterType)) visitor.EnumParameter(info);
            if (IsString(parameterType)) visitor.StringParameter(info);
        }
    }
}
