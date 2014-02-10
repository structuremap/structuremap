using System.Collections.Generic;

namespace StructureMap.Diagnostics
{
    public interface IBulletStyle
    {
        void ApplyBullets(IEnumerable<TabbedLine> lines);
    }
}