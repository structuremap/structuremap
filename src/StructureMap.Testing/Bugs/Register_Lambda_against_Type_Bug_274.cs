using NUnit.Framework;
using Shouldly;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Register_Lambda_against_Type_Bug_274
    {
        [Test]
        public void can_register_the_lambda_against_type()
        {
            var container = new Container(_ => { _.For(typeof (ILetter)).Use("A Letter A", c => new LetterA()); });


            container.GetInstance<ILetter>()
                .ShouldBeOfType<LetterA>();
        }
    }

    public interface ILetter
    {
    }

    public class LetterA : ILetter
    {
    }
}