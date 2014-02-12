namespace StructureMap.Diagnostics
{
    public interface ILeftPadding
    {
        string Create();
        ILeftPadding ToChild(int spaces, string leftBorder = "");
    }
}