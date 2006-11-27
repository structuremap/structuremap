using System;
using System.Data;

namespace StructureMap.DataAccess.JSON
{
	public interface IField
	{
		void Write(JSONObject target, DataRow row);
	}
}
