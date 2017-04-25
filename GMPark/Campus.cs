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
	// location class
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

	// Role class
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

	// campus class
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

		// contructor
		public Campus()
		{
		}

		public Campus(string name)
		{
			Name = name;
		}

		// getters and setter for each variables 
		public List<Role> GetRoles()
		{
			return Roles;
		}

		public string GetName()
		{
			return Name;
		}

		public void SetName(string name)
		{
			Name = name;
		}

		public string GetId()
		{
			return Id;
		}

		public void SetId(string id)
		{
			Id = id;
		}

		// convert to campus obeject
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

		// convert to building obeject
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
		// convert to lot obeject
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

		// get all buildings
		public List<string> GetBuildingList()
		{
			var ls = new List<string>();

			foreach (Building build in Buildings)
			{
				ls.Add(build.GetName());
			}

			return ls;
		}
		// get all building's id
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
		// get lot name
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
		// get lot by id
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

		// add roles from server
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
		// add gate from server
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

		// check if user is in lot now
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

		// get lots locations
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

		// get building entrance
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

		// get role's id
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
