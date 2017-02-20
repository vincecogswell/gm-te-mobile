using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using Xamarin.Forms.GoogleMaps;

namespace GMPark
{
	public class Lot : Struct
	{
		public string ID;
		public bool Employee, Executive, Visitor;
		public List<Location> Locations;
		public float Percentage;
	}
}
