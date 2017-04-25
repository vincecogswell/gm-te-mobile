/* Author : Rob
 * Lot class
 */
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
		private List<int> Accesses = new List<int>(); // accessing hours of a lot
		public List<Location> Locations = new List<Location>(); // lot location
		public float Percentage; // the full percentage of a lot
		private GeoPoly mGeoFence; // geopoly of the lot

		public Lot()
		{
		}

		public Lot(string id)
		{
			SetName(id);
		}

		// check if user is in the lot
		public bool InFence(Plugin.Geolocator.Abstractions.Position pos)
		{
			return mGeoFence.InFence(pos);
		}

		// add border of the lot
		public void AddBorder(Location loc)
		{
			Locations.Add(loc);
		}

		// create geofencing area
		public void CreateGeoFence()
		{
			mGeoFence = new GeoPoly(GetName());

			for (int i = 0; i < Locations.Count(); i++)
			{
				int i2 = (i + 1) % Locations.Count();
				mGeoFence.AddGeoLine(Locations.ElementAt(i), Locations.ElementAt(i2));
			}
		}

		// get all points of the geopoly
		public List<Position> GetPoints()
		{
			var ls = new List<Position>();

			foreach (Location loc in Locations)
			{
				ls.Add(new Position(loc.Lat, loc.Long));
			}
			       
			return ls;
		}

		// set accessing time
		public void SetAccesses(List<int> accesses)
		{
			foreach (int i in accesses)
			{
				Accesses.Add(i);
			}
		}

		// check if the lot is open for now
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

		// navigate user to this lot
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
