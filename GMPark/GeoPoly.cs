/* Author : Rob
 * GeoPoly class
 * Draw on the map, and detect if user is in the polygon
 */
using System;
using System.Collections.Generic;
using Plugin.Geolocator.Abstractions;

namespace GMPark
{
	public class GeoPoly
	{
		private List<GeoLine> mBoundaries;
		private string mName;

		public GeoPoly(string name)
		{
			mBoundaries = new List<GeoLine>();
			mName = name;
		}

		public void AddGeoLine(Location loc1, Location loc2)
		{
			mBoundaries.Add(new GeoLine(loc1, loc2));
		}

		public bool InFence(Position pos)
		{
			var lats = new List<double>();

			foreach (GeoLine line in mBoundaries)
			{
				if (line.InBounds(pos))
				{
					lats.Add(line.GetLat(pos.Longitude));
				}
			}

			if (lats.Count == 0)
			{
				return false;
			}

			lats.Sort();

 			for (int i = 0; i < lats.Count; i = i + 2)
			{
				double minLat = lats[i], maxLat = lats[i + 1];

				if ((pos.Latitude >= minLat) && (pos.Latitude <= maxLat))
				{
					return true;
				}
			}
				
			return false;
		}

		public string getName()
		{
			return mName;
		}
	}
}