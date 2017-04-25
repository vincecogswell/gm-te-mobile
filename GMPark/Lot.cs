using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace GMPark
{
	public class Lot : Struct
	{
		private List<int> Accesses = new List<int>();
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

		public List<Position> GetPoints()
		{
			var ls = new List<Position>();

			foreach (Location loc in Locations)
			{
				ls.Add(new Position(loc.Lat, loc.Long));
			}
			       
			return ls;
		}

		public void SetAccesses(List<int> accesses)
		{
			foreach (int i in accesses)
			{
				Accesses.Add(i);
			}
		}

		public bool InAccesses(int roleId)
		{
			foreach (int i in Accesses)
			{
				if (i == roleId)
				{
					return true;
				}
			}
			return false;
		}

		public void NavigateTo()
		{
			switch (Device.RuntimePlatform)
			{
				case "iOS":
					Device.OpenUri(
						new Uri(string.Format("http://maps.apple.com/?q={0}",
						                      WebUtility.UrlEncode(GetEntrance(0).Lat.ToString() + " " + 
						                                           GetEntrance(0).Long.ToString()))));
					break;

				case "Android":
					Device.OpenUri(
						new Uri(string.Format("geo:0,0?q={0}", WebUtility.UrlEncode(GetEntrance(0).Lat.ToString() +
						                                                            ", " + GetEntrance(0).Long.ToString()))));
					break;
			};
		}
	}
}
