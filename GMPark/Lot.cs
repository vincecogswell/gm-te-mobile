using System;
using System.Collections.Generic;
using System.Linq;
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
		private GeoPoly mGeoFence;

		public Lot()
		{
			mGeoFence = new GeoPoly(ID);
			/*for (int i = 0; i < Locations.Count(); i++)
			{
				int i2 = (i + 1) % Locations.Count();
				mGeoFence.AddGeoLine(Locations.ElementAt(i), Locations.ElementAt(i2));
			}*/
		}

		bool InFence(Plugin.Geolocator.Abstractions.Position pos)
		{
			return mGeoFence.InFence(pos);
		}
	}
}
