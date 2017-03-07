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
	}

	public class Campus
	{
		public string Name;
		public List<Location> Locations;
		public List<Building> Buildings;
		public List<Lot> Lots;
		public List<string> Roles;

		public bool InBoundBox(Position pos)
		{
			double maxLat = -91, maxLong = -181, minLat = 91, minLong = 181;

			foreach (Location location in Locations)
			{
				if (location.Lat < minLat)
				{
					minLat = location.Lat;
				}

				if (location.Lat > maxLat)
				{
					maxLat = location.Lat;
				}

				if (location.Long < minLong)
				{
					minLong = location.Long;
				}

				if (location.Long > maxLong)
				{
					maxLong = location.Long;
				}
			}

			if ((pos.Latitude >= minLat) && (pos.Latitude <= maxLat) &&
				(pos.Longitude >= minLong) && (pos.Longitude <= maxLong))
			{
				return true;
			}

			else
			{
				return false;
			}
		}
	}
}
