using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class InjectByFuncWithNoPublicConstructors
    {
        [Fact]
        public void register_with_generic()
        {
            var container =
                new Container(x => { x.For<ClassThatIsBuiltByStatic>().Use(c => ClassThatIsBuiltByStatic.Build()); });

            container.GetInstance<ClassThatIsBuiltByStatic>().ShouldNotBeNull();
        }
    }

    public class ClassThatIsBuiltByStatic
    {
        private ClassThatIsBuiltByStatic()
        {
        }

        public static ClassThatIsBuiltByStatic Build()
        {
            return new ClassThatIsBuiltByStatic();
        }
    }
}