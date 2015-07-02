using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class enumerable_instances
    {
        // SAMPLE: EnumerableFamilyPolicy_in_action
        [Test]
        public void collection_types_are_all_possible_by_default()
        {
            // NOTE that we do NOT make any explicit registration of 
            // IList<IWidget>, IEnumerable<IWidget>, ICollection<IWidget>, or IWidget[]
            var container = new Container(_ =>
            {
                _.For<IWidget>().Add<AWidget>();
                _.For<IWidget>().Add<BWidget>();
                _.For<IWidget>().Add<CWidget>();
            });

            // IList<T>
            container.GetInstance<IList<IWidget>>()
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof (AWidget), typeof (BWidget), typeof (CWidget));

            // ICollection<T>
            container.GetInstance<ICollection<IWidget>>()
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof (AWidget), typeof (BWidget), typeof (CWidget));

            // Array of T
            container.GetInstance<IWidget[]>()
                .Select(x => x.GetType())
                .ShouldHaveTheSameElementsAs(typeof (AWidget), typeof (BWidget), typeof (CWidget));
        }

        // ENDSAMPLE
    }
}