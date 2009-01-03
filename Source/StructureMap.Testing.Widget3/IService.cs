namespace StructureMap.Testing.Widget3
{
    public interface IService : IBasicService
    {
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

        public string Color
        {
            get { return _color; }
        }


        public override string ToString()
        {
            return "ColorService:  " + _color;
        }
    }
}