using System;

namespace StructureMap.Testing.Widget3
{
    public interface IService : IBasicService
    {
        void DoSomething();
    }

    public interface IBasicService
    {
    }


    public class ColorService : IService
    {
        private readonly string _color;

        public ColorService(string color)
        {
            _color = color;
        }

        public string Color { get { return _color; } }


        public override string ToString()
        {
            return "ColorService:  " + _color;
        }

        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }

    public class WhateverService : IService
    {
        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }
}