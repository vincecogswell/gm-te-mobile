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

	public class ServerJSONBuildings
	{
		public string status;
		public string message;
		public Dictionary<string, SBuilding> buildings;

		public ServerJSONBuildings()
		{
		}
	}

	public class SBuilding
	{
		public List<List<string>> entrances;
		public string name;
		public bool active;
	}

	public class ServerJSONLots
	{
		public string status;
		public string message;
		public Dictionary<string, SLot> lots;

		public ServerJSONLots()
		{
		}
	}

	public class SLot
	{
		public List<List<string>> entrances;
		public List<List<string>> perimeter;
		public List<int> access;
		public string start;
		public string end;
		public string name;
		public bool active;
	}

	public class ServerJSONRoles
	{
		public string status;
		public string message;
		public List<SRole> Roles;

		public ServerJSONRoles()
		{
		}
	}

	public class SRole
	{
		public string name;
		public int id;
	}

	public class ServerJSONGates
	{
		public string status;
		public string message;
		public Dictionary<string, SGate> gates;

		public ServerJSONGates()
		{
		}
	}

	public class SGate
	{
		public string name;
		public List<int> tempAccess;
		public List<List<string>> location;
		public string start;
		public string end;
		public bool active;
	}

	public class ServerJSONLotOrder
	{
		public string status;
		public string message;
		public List<int> lot_order;
	}

}
