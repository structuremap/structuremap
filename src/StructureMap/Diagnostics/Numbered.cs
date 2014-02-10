using System.Collections.Generic;
using System.Linq;

namespace StructureMap.Diagnostics
{
    public class Numbered : IBulletStyle
    {
        public void ApplyBullets(IEnumerable<TabbedLine> lines)
        {
            var spaces = lines.Count().ToString().Length + 3;

            var i = 0;
            lines.Each(line => {
                i++;

                line.Bullet = (i + ".) ").PadLeft(spaces);
            });
        }
    }
}