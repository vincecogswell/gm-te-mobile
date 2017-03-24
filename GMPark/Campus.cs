using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using Xamarin.Forms.GoogleMaps;

namespace GMPark
{
	public class Location
	{
		public double Lat, Long;

		public Location(double nLat, double nLong)
		{
			Lat = nLat;
			Long = nLong;
		}

		public Location()
		{
		}
	}

	public class Campus
	{
		public string Name;
		public List<Location> Locations;
		public List<Building> Buildings;
		public List<Lot> Lots;
		public List<string> Roles;

		public Campus()
		{
			Locations = new List<Location>();
		}

		public Campus(string name)
		{
			Name = name;
		}

		public string GetName()
		{
			return Name;
		}

		public void SetName(string name)
		{
			Name = name;
		}

		public void ConvertToCampus(SCampus server)
		{
			foreach (List<string> ls in server.perimeter)
			{
				double lat = Convert.ToDouble(ls[0]), lon = Convert.ToDouble(ls[1]);
				var loc = new Location(lat, lon);
				Locations.Add(loc);
			}

			SetName(server.name);
				
		}
			
	}

}
