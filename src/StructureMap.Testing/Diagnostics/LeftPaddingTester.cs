using NUnit.Framework;
using StructureMap.Diagnostics;

namespace StructureMap.Testing.Diagnostics
{
    [TestFixture]
    public class LeftPaddingTester
    {
        [Test]
        public void with_no_border()
        {
            new LeftPadding(5).Create()
                .ShouldEqual("     ");
        }

        [Test]
        public void with_a_border()
        {
            new LeftPadding(5, "|").Create()
                .ShouldEqual("|    ");
        }

        [Test]
        public void to_child()
        {
            new LeftPadding(5, "|")
                .ToChild(3, "*")
                .ToChild(3, "&&")
                .Create()
                .ShouldEqual("|    *  && ");
                            //|^^^^*^^&&^ 
        }
    }
}