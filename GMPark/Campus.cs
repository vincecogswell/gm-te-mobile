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
	}
}
