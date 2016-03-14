using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class ScanIndexerBugTester
    {
        [Fact]
        public void do_not_blow_up_on_scanning_the_property_for_indexer()
        {
            var container = new Container();
            container.GetInstance<ClassWithIndexer>().ShouldNotBeNull();
        }
    }

    public class ClassWithIndexer
    {
        public string this[string key]
        {
            get { return key; }
            set { }
        }
    }
}