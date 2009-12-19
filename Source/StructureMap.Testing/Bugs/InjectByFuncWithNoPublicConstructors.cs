using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class InjectByFuncWithNoPublicConstructors
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void register_with_generic()
        {
            var container = new Container(x =>
            {
                x.For<ClassThatIsBuiltByStatic>().Use(c => ClassThatIsBuiltByStatic.Build());
            });

            container.GetInstance<ClassThatIsBuiltByStatic>().ShouldNotBeNull();
        }
    }

    public class ClassThatIsBuiltByStatic
    {
        public static ClassThatIsBuiltByStatic Build()
        {
            return new ClassThatIsBuiltByStatic();
        }

        private ClassThatIsBuiltByStatic(){}
    }
}