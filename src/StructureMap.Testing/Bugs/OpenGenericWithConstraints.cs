using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    public class ClosedGenericForStruct<T> : IAmOpenGeneric<T>
        where T : struct
    {
    }

    public class ClosedGenericForEnumerable<T> : IAmOpenGeneric<T>
        where T : IEnumerable
    {
    }

    public struct EnumerableStruct : IEnumerable
    {
        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    [TestFixture]
    public class OpenGenericWithConstraints
    {
        [Test]
        public void RegisterTwoInheritorsWithDifferentTypeConstraints()
        {
            var container = new Container(x => {
                x.Scan(y => y.TheCallingAssembly());
                x.For(typeof (IAmOpenGeneric<>)).Add(typeof (ClosedGenericForEnumerable<>));
                x.For(typeof (IAmOpenGeneric<>)).Add(typeof (ClosedGenericForStruct<>));
            });

            container.GetInstance<IAmOpenGeneric<int>>().ShouldBeOfType<ClosedGenericForStruct<int>>();
            container.GetInstance<IAmOpenGeneric<ArrayList>>().ShouldBeOfType<ClosedGenericForEnumerable<ArrayList>>();

            IEnumerable<IAmOpenGeneric<EnumerableStruct>> amOpenGenerics =
                container.GetAllInstances<IAmOpenGeneric<EnumerableStruct>>();
            amOpenGenerics.Single(x => x.GetType() == typeof (ClosedGenericForStruct<EnumerableStruct>));
            amOpenGenerics.Single(x => x.GetType() == typeof (ClosedGenericForEnumerable<EnumerableStruct>));

            amOpenGenerics.Count().ShouldEqual(2);
        }
    }
}