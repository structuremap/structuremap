using System;
using System.Collections.Generic;
using System.Linq;

namespace StructureMap.Diagnostics
{
    public class OutlineWithBars : IBulletStyle
    {
        public void ApplyBullets(IEnumerable<TabbedLine> lines)
        {
            if (lines.Any())
            {
                lines.Each(x => x.Bullet = Convert.ToChar(9507) + " ");
                lines.Last().Bullet = Convert.ToChar(9495) + " ";
            }
        }
    }
}