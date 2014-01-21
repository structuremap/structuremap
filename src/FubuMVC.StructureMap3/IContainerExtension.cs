using StructureMap;

namespace FubuMVC.StructureMap3
{
    // TODO -- this needs to be in SM3 itself
    public interface IContainerExtension
    {
        void Extend(IContainer container);
    }
}