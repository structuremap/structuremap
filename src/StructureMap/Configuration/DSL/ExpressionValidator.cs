using System;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace StructureMap.Configuration.DSL
{
    public static class ExpressionValidator
    {
        public static ValidateExpression ValidatePluggabilityOf(Type TPluggedType)
        {
            return new ValidateExpression(TPluggedType);
        }

        #region Nested type: ValidateExpression

        public class ValidateExpression
        {
            private readonly Type _TPluggedType;

            public ValidateExpression(Type TPluggedType)
            {
                _TPluggedType = TPluggedType;
            }

            public void IntoPluginType(Type pluginType)
            {
                if (!Constructor.HasConstructors(_TPluggedType))
                {
                    throw new StructureMapException(180, _TPluggedType.AssemblyQualifiedName);
                }

                if (!_TPluggedType.CanBeCastTo(pluginType))
                {
                    throw new StructureMapException(
                        303,
                        _TPluggedType.AssemblyQualifiedName,
                        pluginType.AssemblyQualifiedName);
                }
            }
        }

        #endregion
    }
}