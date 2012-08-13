using System;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace StructureMap.Configuration.DSL
{
    public static class ExpressionValidator
    {
        public static ValidateExpression ValidatePluggabilityOf(Type pluggedType)
        {
            return new ValidateExpression(pluggedType);
        }

        #region Nested type: ValidateExpression

        public class ValidateExpression
        {
            private readonly Type _pluggedType;

            public ValidateExpression(Type pluggedType)
            {
                _pluggedType = pluggedType;
            }

            public void IntoPluginType(Type pluginType)
            {
                if (!Constructor.HasConstructors(_pluggedType))
                {
                    throw new StructureMapException(180, _pluggedType.AssemblyQualifiedName);
                }

                if (!_pluggedType.CanBeCastTo(pluginType))
                {
                    throw new StructureMapException(
                        303,
                        _pluggedType.AssemblyQualifiedName,
                        pluginType.AssemblyQualifiedName);
                }
            }
        }

        #endregion
    }
}