using StructureMap.Configuration.Tokens;
using StructureMap.Configuration.Tokens.Properties;

namespace StructureMap.Configuration
{
    public interface IConfigurationVisitor
    {
        void StartObject(GraphObject node);
        void EndObject(GraphObject node);

        void HandleAssembly(AssemblyToken assembly);
        void HandleFamily(FamilyToken family);
        void HandleMementoSource(MementoSourceInstanceToken source);
        void HandlePlugin(PluginToken plugin);
        void HandleInterceptor(InterceptorInstanceToken interceptor);
        void HandleInstance(InstanceToken instance);
        void HandlePrimitiveProperty(PrimitiveProperty property);
        void HandleEnumerationProperty(EnumerationProperty property);
        void HandleInlineChildProperty(ChildProperty property);
        void HandleDefaultChildProperty(ChildProperty property);
        void HandleReferenceChildProperty(ChildProperty property);
        void HandlePropertyDefinition(PropertyDefinition propertyDefinition);
        void HandleChildArrayProperty(ChildArrayProperty property);
        void HandleNotDefinedChildProperty(ChildProperty property);
        void HandleTemplate(TemplateToken template);
        void HandleTemplateProperty(TemplateProperty property);
    }
}