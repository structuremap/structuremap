namespace StructureMap.Testing.Widget3
{
	public interface IService
	{
	}


	public class ColorService : IService
	{
		private readonly string _color;

		public ColorService(string Color)
		{
			_color = Color;
		}

		public string Color
		{
			get { return _color; }
		}
	}

}