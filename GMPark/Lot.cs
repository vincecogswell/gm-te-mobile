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
		public bool Employee, Executive, Visitor;
		public List<Location> Locations = new List<Location>();
		public float Percentage;
		private GeoPoly mGeoFence;

		public Lot()
		{
			/*for (int i = 0; i < Locations.Count(); i++)
			{
				int i2 = (i + 1) % Locations.Count();
				mGeoFence.AddGeoLine(Locations.ElementAt(i), Locations.ElementAt(i2));
			}*/

		}

		public Lot(string id)
		{
			SetName(id);
		}

		public bool InFence(Plugin.Geolocator.Abstractions.Position pos)
		{
			return mGeoFence.InFence(pos);
		}

		public void AddBorder(Location loc)
		{
			Locations.Add(loc);
		}

		public void CreateGeoFence()
		{
			mGeoFence = new GeoPoly(GetName());

			for (int i = 0; i < Locations.Count(); i++)
			{
				int i2 = (i + 1) % Locations.Count();
				mGeoFence.AddGeoLine(Locations.ElementAt(i), Locations.ElementAt(i2));
			}
		}
	}
}
