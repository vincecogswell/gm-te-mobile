/* Campus class
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using Xamarin.Forms.GoogleMaps;

namespace GMPark
{
	// location class, holds latitude and longitude
	public class Location
	{
		// varibles
		public double Lat, Long;
		//constructor
		public Location(double nLat, double nLong)
		{
			Lat = nLat;
			Long = nLong;
		}
		public Location()
		{
		}
	}

	// Role class, holds name and id
	public class Role
	{
		// variables
		private string name;
		private int id;

		// get role name
		public string GetName()
		{
			return name;
		}
		// set role name
		public void SetName(string Name)
		{
			name = Name;
		}
		// get id
		public int GetId()
		{
			return id;
		}
		// set id
		public void SetId(int Id)
		{
			id = Id;
		}
	}

	/* Campus class that holds all of the objects such as the lots, roles, buildings, and gates associated with it
	 */
	public class Campus
	{
		// variables
		public string Name;
		public List<Location> Locations = new List<Location>();
		public List<Building> Buildings = new List<Building>();
		public List<Lot> Lots = new List<Lot>();
		public List<Role> Roles = new List<Role>();
		public List<Gate> Gates = new List<Gate>();
		private string Id;

		/* Constructor
		 */
		public Campus()
		{
		}

		/* Constructor
		 * Parameters: string (name of the campus)
		 */
		public Campus(string name)
		{
			Name = name;
		}

		/* Getter of the roles of the campus
		* Returns: List<Role> (all the roles associated with the campus)
		*/
		public List<Role> GetRoles()
		{
			return Roles;
		}

		/* Getter of the name of the campus
		 * Returns: string (name of the campus)
		 */
		public string GetName()
		{
			return Name;
		}

		/* Setter of the name of the campus
		 * Parameters: string (name of the campus)
		 */
		public void SetName(string name)
		{
			Name = name;
		}

		/* Getter of the id of the campus
 		 * Returns: string (id of the campus)	 */
		public string GetId()
		{
			return Id;
		}

		/* Setter of the id of the campus
 		 * Parameters: string (id of the campus)	 */
		public void SetId(string id)
		{
			Id = id;
		}

		/* Takes in a campus object from the server and converts it to a campus
		 * Parameters: SCampus (campus object from server to be converted)
		 */
		public void ConvertToCampus(SCampus server)
		{
			if (server.perimeter.Count == 2)
			{
				double lat1 = Convert.ToDouble(server.perimeter[0][0]),
				lon1 = Convert.ToDouble(server.perimeter[0][1]),
				lat2 = Convert.ToDouble(server.perimeter[1][0]),
				lon2 = Convert.ToDouble(server.perimeter[1][1]);

				var loc1 = new Location(lat1, lon1);
				var loc2 = new Location(lat2, lon1);
				var loc3 = new Location(lat2, lon2);
				var loc4 = new Location(lat1, lon2);

				Locations.Add(loc1);
				Locations.Add(loc2);
				Locations.Add(loc3);
				Locations.Add(loc4);
			}

			else
			{
				foreach (List<string> ls in server.perimeter)
				{
					double lat = Convert.ToDouble(ls[0]), lon = Convert.ToDouble(ls[1]);
					var loc = new Location(lat, lon);
					Locations.Add(loc);
				}
			}

			SetName(server.name);
		}

		/* Takes in a buildings object from the server and adds the buildings to the camous
		 * Parameters: ServerJSONBuildings (buildings object from server to be added)
		 */
		public void ConvertBuildings(ServerJSONBuildings server)
		{
			foreach (KeyValuePair<string, SBuilding> entry in server.buildings)
			{
				var build = new Building(entry.Value.name);
				build.SetActive(entry.Value.active);
				build.SetId(entry.Key);

				foreach (List<string> ls in entry.Value.entrances)
				{
					double lat = Convert.ToDouble(ls[0]), lon = Convert.ToDouble(ls[1]);
					var loc = new Location(lat, lon);
					build.AddEntrance(loc);
				}

				Buildings.Add(build);
			}

		}
		/* Takes in a lots object from the server and adds the lots to the campus
		 * Parameters: ServerJSONLots (lots object from server to be added)
		 */
		public void ConvertLots(ServerJSONLots server)
		{
			foreach (KeyValuePair<string, SLot> entry in server.lots)
			{
				var lot = new Lot(entry.Value.name);
				lot.SetActive(entry.Value.active);
				lot.SetId(entry.Key);

				foreach (List<string> ls in entry.Value.entrances)
				{
					double lat = Convert.ToDouble(ls[0]), lon = Convert.ToDouble(ls[1]);
					var loc = new Location(lat, lon);
					lot.AddEntrance(loc);
				}

				lot.SetAccesses(entry.Value.access);

				if (entry.Value.perimeter.Count == 2)
				{
					double lat1 = Convert.ToDouble(entry.Value.perimeter[0][0]),
					lon1 = Convert.ToDouble(entry.Value.perimeter[0][1]),
					lat2 = Convert.ToDouble(entry.Value.perimeter[1][0]),
					lon2 = Convert.ToDouble(entry.Value.perimeter[1][1]);

					var loc1 = new Location(lat1, lon1);
					var loc2 = new Location(lat2, lon1);
					var loc3 = new Location(lat2, lon2);
					var loc4 = new Location(lat1, lon2);

					lot.AddBorder(loc1);
					lot.AddBorder(loc2);
					lot.AddBorder(loc3);
					lot.AddBorder(loc4);
					lot.CreateGeoFence();
				}

				else
				{
					foreach (List<string> ls in entry.Value.perimeter)
					{
						double lat = Convert.ToDouble(ls[0]), lon = Convert.ToDouble(ls[1]);
						var loc = new Location(lat, lon);
						lot.AddBorder(loc);
						lot.CreateGeoFence();
					}
				}
				Lots.Add(lot);
			}
		}

		/* Gets a list of all the names of the buildings on the campus
		 * Returns: List<string> (names of all the buildings on campus)
		 */
		public List<string> GetBuildingList()
		{
			var ls = new List<string>();

			foreach (Building build in Buildings)
			{
				ls.Add(build.GetName());
			}

			return ls;
		}

		/* Gets the id of a building based on the name of the building passed in
		 * Parameters: string (name of the building)
		 * Returns: string (id of the building)
		 */
		public string GetBuildingId(string buildingName)
		{
			foreach (Building build in Buildings)
			{
				if (build.GetName() == buildingName)
				{
					return build.GetId();
				}
			}

			return null;
		}

		/* Gets the name of a lot based on the id of the lot passed in
		 * Parameters: string (id of the lot)
		 * Returns: string (name of the lot)
		 */
		public string GetLotName(string lotId)
		{
			foreach (Lot lot in Lots)
			{
				if (lotId == lot.GetId())
				{
					return lot.GetName();
				}
			}
			return null;
		}

		/* Gets the lot object based on the id of the lot passed in
		 * Parameters: string (id of the lot)
		 * Returns: Lot (lot object to be returned)
		 */
		public Lot GetLotById(string lotId)
		{
			foreach (Lot lot in Lots)
			{
				if (lot.GetId() == lotId)
				{
					return lot;
				}
			}
			return null;
		}

		/* Takes in a roles object from the server and adds the roles to the campus
		 * Parameters: ServerJSONRoles (roles object from server to be added)
		 */
		public void AddRoles(ServerJSONRoles server)
		{

			foreach (SRole role in server.Roles)
			{
				var r = new Role();
				r.SetId(role.id);
				r.SetName(role.name);
				Roles.Add(r);
			}
		}

		/* Takes in a gates object from the server and adds the gates to the campus
		 * Parameters: ServerJSONGates (gates object from server to be added)
		 */
		public void AddGates(ServerJSONGates server)
		{
			foreach (KeyValuePair<string, SGate> entry in server.gates)
			{
				var gate = new Gate();
				gate.SetId(entry.Key);
				gate.SetName(entry.Value.name);
				gate.SetActive(entry.Value.active);
				gate.SetTime(entry.Value.start, entry.Value.end);
				gate.AddEntrance(new Location(Convert.ToDouble(entry.Value.location[0][0]),
											  Convert.ToDouble(entry.Value.location[0][1])));

				foreach (List<string> instructionSet in entry.Value.tempAccess)
				{
					gate.AddInstruction(instructionSet[0], instructionSet[1]);
				}

				Gates.Add(gate);
			}
		}

		/* Checks to see which lot the position is in
         * Parameters: Plugin.Geolocator.Abstractions.Position (position to be checked)
         * Returns: string (name of the lot that the position is in)
         */
		public string CheckInLotGeoFences(Plugin.Geolocator.Abstractions.Position pos)
		{
			foreach (Lot lot in Lots)
			{
				if (lot.InFence(pos))
				{
					return lot.GetName();
				}
			}

			return "";
		}

		/* Checks to see if the position is in any of the lots
		 * Parameters: Plugin.Geolocator.Abstractions.Position (position to be checked)
		 * Returns: bool (true if in a lot geofence, false if not) 
		 */
		public bool CheckIfInLotGeoFences(Plugin.Geolocator.Abstractions.Position pos)
		{
			foreach (Lot lot in Lots)
			{
				if (lot.InFence(pos))
				{
					return true;
				}
			}

			return false;
		}

		/* Gets the points of the lot based on the id passed
		 * Parameters: string (the id of the lot)
		 * Returns: List<Position> (the boundaries of the lot)
		 */
		public List<Position> GetLotPoints(string lotId)
		{
			foreach (Lot lot in Lots)
			{
				if (lot.GetId() == lotId)
				{
					return lot.GetPoints();
				}
			}

			return null;
		}

		/* Gets the entrances of the lot based on the id passed
		 * Parameters: string (the id of the lot)
		 * Returns: List<Position> (the entrances of the lot)
		 */
		public List<Position> GetBuildingEntrances(string buildingName)
		{
			foreach (Building build in Buildings)
			{
				if (build.GetName() == buildingName)
				{
					return build.GetEntrances();
				}
			}

			return null;
		}

		/* Getter of the role id
		 * Parameters: the name of the role to get the id for
		 * Returns: string (id of the role)
		 */
		public int GetRoleId(string roleName)
		{
			foreach (Role role in Roles)
			{
				if (role.GetName() == roleName)
				{
					return role.GetId();
				}
			}
			return -1;
		}
		/* Gets rid of all the lots in the lotOrder that are not accessable by the role id passed
		 * Parameters: int (id of the role), List<int> (list of lot Ids)
         * Returns: List<int> (list of lots that are accessible to the role)
         */
		public List<int> PurgeLotOrder(int roleId, List<int> lotOrder)
		{
			List<int> lots = new List<int>();

			foreach (Lot lot in Lots)
			{
				if (lot.InAccesses(roleId))
				{
					lots.Add(Convert.ToInt32(lot.GetId()));
				}

				else
				{
				}
			}

			return lots;
		}
					
	}

}
