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

		/* Constructor
		 */
		public Lot()
		{
		}
		/* Constructor
		 * Parameters: string (id of the lot)
		 */
		public Lot(string id)
		{
			SetName(id);
		}

		/* Checks to see if position is in geofence of lot
         * Parameters: Plugin.Geolocator.Abstractions.Position (position to be checked)
         * Returns: bool (true if in lot, false if not)
         */
		public bool InFence(Plugin.Geolocator.Abstractions.Position pos)
		{
			return mGeoFence.InFence(pos);
		}

		/* Adds a border to the lot
		 * Parameters: Location (border of lot)
		 */
		public void AddBorder(Location loc)
		{
			Locations.Add(loc);
		}

		/* Creates the geofence of the lot
		 */
		public void CreateGeoFence()
		{
			mGeoFence = new GeoPoly(GetName());

			for (int i = 0; i < Locations.Count(); i++)
			{
				int i2 = (i + 1) % Locations.Count();
				mGeoFence.AddGeoLine(Locations.ElementAt(i), Locations.ElementAt(i2));
			}
		}

		/* Returns a list of all the boundaries of the lot
		 * Returns: List<Position> (boundaries of the lot)
		 */
		public List<Position> GetPoints()
		{
			var ls = new List<Position>();

			foreach (Location loc in Locations)
			{
				ls.Add(new Position(loc.Lat, loc.Long));
			}
			       
			return ls;
		}

		/* Sets which roles can access the lot
		 * Parameters: List<int> (list of roleids who can access the lot)
		 */
		public void SetAccesses(List<int> accesses)
		{
			foreach (int i in accesses)
			{
				Accesses.Add(i);
			}
		}

		/* Checks to see if lot is accessible to the roleId
		 * Parameters: int (the id of the role to be checked)
		 * Returns: bool (true if lot can be accessed, false if not)
		 */
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

		/* Opens up a navigation application and directs the user to the first entrance of the lotr
		 */
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
