using System;
using System.Text;

namespace StructureMap.Configuration
{
	[Serializable]
	public class Problem
	{
		private string _title;
		private string _message;
		private string _path = string.Empty	;
		private Guid _objectId = Guid.Empty;

		public Problem()
		{

		}

		public Problem(string title, string message)
		{
			_title = title;
			_message = message;
		}

		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}

		public Problem(string title, Exception ex)
		{
			_title = title;
	
			StringBuilder sb = new StringBuilder();
			Exception exception = ex;
			while (exception != null)
			{
				sb.Append("\n");
				sb.Append(exception.Message);
				sb.Append("\n");
				sb.Append(exception.StackTrace);

				exception = exception.InnerException;
			}

			_message = sb.ToString();
		}

		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		public Guid ObjectId
		{
			get { return _objectId; }
			set { _objectId = value; }
		}

		public override string ToString()
		{
			return string.Format("Problem:  {0}\n{1}", this.Title, this.Message);
		}

		public override bool Equals(object obj)
		{
			Problem peer = obj as Problem;
			if (peer == null)
			{
				return false;
			}

			return this.Title == peer.Title;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
