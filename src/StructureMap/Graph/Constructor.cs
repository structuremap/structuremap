using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using StructureMap.TypeRules;

namespace StructureMap.Graph
{
    public class Constructor
    {
        private readonly Type _TPluggedType;
        private ConstructorInfo _ctor;

        public Constructor(Type TPluggedType)
        {
            _TPluggedType = TPluggedType;
            _ctor = GetConstructor(TPluggedType);
        }

        public ConstructorInfo Ctor { get { return _ctor; } }

        /// <summary>
        /// Returns the System.Reflection.ConstructorInfo for the PluggedType.  Uses either
        /// the "greediest" constructor with the most arguments or the constructor function
        /// marked with the [DefaultConstructor]
        /// </summary>
        /// <returns></returns>
        public static ConstructorInfo GetConstructor(Type TPluggedType)
        {
            ConstructorInfo returnValue = DefaultConstructorAttribute.GetConstructor(TPluggedType);

            // if no constructor is marked as the "ContainerConstructor", find the greediest constructor
            if (returnValue == null)
            {
                returnValue = GetGreediestConstructor(TPluggedType);
            }

            return returnValue;
        }

        public static bool HasConstructors(Type TPluggedType)
        {
            return GetGreediestConstructor(TPluggedType) != null;
        }

        public static ConstructorInfo GetGreediestConstructor(Type TPluggedType)
        {
            ConstructorInfo returnValue = null;

            foreach (ConstructorInfo constructor in TPluggedType.GetConstructors())
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

        public bool CanBeAutoFilled()
        {
            if (_ctor == null) return false;

            foreach (ParameterInfo parameter in _ctor.GetParameters())
            {
                if (!parameter.ParameterType.IsAutoFillable())
                {
                    return false;
                }
            }

            return true;
        }

        public string FindFirstConstructorArgumentOfType(Type type)
        {
            return _ctor.GetParameters()
                .Where(x => x.ParameterType == type)
                .Select(x => x.Name).FirstOrDefault();
        }


        public void Visit(IArgumentVisitor visitor)
        {
            try
            {
                foreach (ParameterInfo info in _ctor.GetParameters())
                {
                    try
                    {
                        Type parameterType = info.ParameterType;
                        visitParameter(info, parameterType, visitor);
                    }
                    catch (Exception e)
                    {
                        string message =
                            "Trying to visit parameter {0} of type {1} in the constructor for {2}"
                                .ToFormat(info.Name, info.ParameterType, _TPluggedType.AssemblyQualifiedName);
                        throw new ApplicationException(message, e);
                    }
                }
            }
            catch (Exception e)
            {
                string message = "Failed while trying to visit the constructor for " +
                                 _TPluggedType.AssemblyQualifiedName;
                throw new ApplicationException(message, e);
            }
        }

        private void visitParameter(ParameterInfo info, Type parameterType, IArgumentVisitor visitor)
        {
            if (parameterType.IsPrimitive()) visitor.PrimitiveParameter(info);
            if (parameterType.IsChild()) visitor.ChildParameter(info);
            if (parameterType.IsChildArray()) visitor.ChildArrayParameter(info);
            if (parameterType.IsEnum) visitor.EnumParameter(info);
            if (parameterType.IsString()) visitor.StringParameter(info);
        }

        public bool HasArguments()
        {
            if (_ctor == null) return false;
            return _ctor.GetParameters().Length > 0;
        }

        public bool IsValid()
        {
            return _ctor != null;
        }

        public void UseConstructor(Expression expression)
        {
            var finder = new ConstructorFinderVisitor();
            finder.Visit(expression);

            ConstructorInfo ctor = finder.Constructor;
            if (ctor == null)
            {
                throw new ApplicationException("Not a valid constructor function");
            }

            _ctor = ctor;
        }

        public Type FindArgumentType(string name)
        {
            return _ctor.GetParameters()
                .Where(x => x.Name == name)
                .Select(x => x.ParameterType).FirstOrDefault();
        }
    }
}