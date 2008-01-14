using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Testing.Widget4;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class ChildInstanceExpressionTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test,
         ExpectedException(typeof (StructureMapException),
             "StructureMap Exception Code:  303\nType System.String,mscorlib is either abstract or cannot be plugged into Type StructureMap.Testing.Configuration.DSL.IType,StructureMap.Testing"
             )]
        public void CantCastTheRequestedConcreteType()
        {
            InstanceExpression instance = new InstanceExpression(typeof (IStrategy));
            MemoryInstanceMemento memento = new MemoryInstanceMemento();

            ChildInstanceExpression expression =
                new ChildInstanceExpression(instance, memento, "a property", typeof (IType));
            expression.IsConcreteType<string>();
        }

        [Test,
         ExpectedException(typeof (StructureMapException),
             "StructureMap Exception Code:  303\nType StructureMap.Testing.Configuration.DSL.AbstractType,StructureMap.Testing is either abstract or cannot be plugged into Type StructureMap.Testing.Configuration.DSL.IType,StructureMap.Testing"
             )]
        public void CantCastTheRequestedConcreteType2()
        {
            InstanceExpression instance = new InstanceExpression(typeof (IStrategy));
            MemoryInstanceMemento memento = new MemoryInstanceMemento();

            ChildInstanceExpression expression =
                new ChildInstanceExpression(instance, memento, "a property", typeof (IType));
            expression.IsConcreteType<AbstractType>();
        }


        [Test,
         ExpectedException(typeof (StructureMapException),
             "StructureMap Exception Code:  303\nType StructureMap.Testing.Configuration.DSL.AbstractType,StructureMap.Testing is either abstract or cannot be plugged into Type StructureMap.Testing.Configuration.DSL.IType,StructureMap.Testing"
             )]
        public void CantCastTheRequestedPluggedType3()
        {
            InstanceExpression instance = new InstanceExpression(typeof (IStrategy));
            MemoryInstanceMemento memento = new MemoryInstanceMemento();

            ChildInstanceExpression expression =
                new ChildInstanceExpression(instance, memento, "a property", typeof (IType));
            InstanceExpression child = Registry.Instance<IType>().UsingConcreteType<AbstractType>();

            expression.Is(child);
        }
    }

    public interface IType
    {
    }

    public abstract class AbstractType : IType
    {
    }

    public class ConcreteType : AbstractType
    {
    }
}