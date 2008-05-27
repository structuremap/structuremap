using System;
using System.Collections.Generic;
using System.Text;

namespace StructureMap
{
    public delegate void Action();
    public delegate void Action<T>(T t);
    public delegate void Action<T, T1>(T t, T1 t1);
    public delegate T Func<T>();
    public delegate T1 Func<T, T1>(T t);
    
}
