namespace StructureMap.Configuration.Tokens.Properties
{
    public interface IChildPropertyMode
    {
        void Validate(IInstanceValidator validator);
        void AcceptVisitor(IConfigurationVisitor visitor);
    }
}