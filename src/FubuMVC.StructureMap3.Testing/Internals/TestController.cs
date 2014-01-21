using System;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.StructureMap3.Testing.Internals
{
    public class TestViewModel
    {
        public bool BoolProperty { get; set; }
        public string StringProperty { get; set; }
        public int IntProperty { get; set; }
    }

    public class TestController
    {
        public TestOutputModel Index()
        {
            return new TestOutputModel();
        }

        public TestOutputModel SomeAction(TestInputModel value)
        {
            return new TestOutputModel
            {
                Prop1 = value.Prop1
            };
        }

        public TestOutputModel SomeAction(int not_used)
        {
            return new TestOutputModel();
        }

        public TestOutputModel2 AnotherAction(TestInputModel value)
        {
            return new TestOutputModel2
            {
                Prop1 = value.Prop1,
                Name = value.Name,
                Age = value.Age
            };
        }

        public TestOutputModel3 ThirdAction(TestInputModel value)
        {
            return new TestOutputModel3
            {
                Prop1 = value.Prop1
            };
        }

        public void RedirectAction(TestInputModel value)
        {
        }
    }

    public class TestInputModel
    {
        public int PropInt { get; set; }
        public string Prop1 { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class TestOutputModel
    {
        public string Prop1 { get; set; }
    }

    public class TestOutputModel2 : TestOutputModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class TestOutputModel3 : TestOutputModel
    {
    }

    public class TestPartialModel
    {
        public string PartialModelProp1 { get; set; }
    }

    public class TestBehavior2 : IActionBehavior
    {
        public IActionBehavior InsideBehavior { get; set; }

        #region IActionBehavior Members

        public void Invoke()
        {
            throw new NotImplementedException();
        }

        public void InvokePartial()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class TestBehavior : IActionBehavior
    {
        public IActionBehavior InsideBehavior { get; set; }

        #region IActionBehavior Members

        public void Invoke()
        {
            throw new NotImplementedException();
        }

        public void InvokePartial()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}