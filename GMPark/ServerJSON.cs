using System;
using System.Collections.Generic;
using System.Linq;

namespace GMPark
{
	public class ServerJSON
	{
		public string status;
		public string message;
		public Dictionary<string, SCampus> campuses;

		public ServerJSON()
		{
		}
	}

	public class SCampus
	{
		public List<List<string>> perimeter;
		public int num_gates;
		public string name;
		public int num_lots;
		public int num_buildings;
		public bool active;
	}

}
