/* Author : Rob
 * GeoPoly class
 * GeoFencing object that is made of GeoLines
 */
using System;
using System.Collections.Generic;
using Plugin.Geolocator.Abstractions;

namespace GMPark
{
	/* This is the core class of the Geofencing algorithm, and is made up of geolines
	 */
	public class GeoPoly
	{
		private List<GeoLine> mBoundaries;
		private string mName;

		/* Constructor
		 * Parameters: string (name of the GeoPoly)
		 */
		public GeoPoly(string name)
		{
			mBoundaries = new List<GeoLine>();
			mName = name;
		}

		/* Adds a line to the GeoPoly based on the two locations passed in
		 * Parameters: Location (line start), Location (line end)
		 */
		public void AddGeoLine(Location loc1, Location loc2)
		{
			mBoundaries.Add(new GeoLine(loc1, loc2));
		}

		/* Checks to see if the position is in the GeoPoly
		 * Parameters: Position (the position to be checked)
		 */
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

		/* Getter of mName
		 * Returns: string (mName of the GeoPoly object)
		 */
		public string getName()
		{
			return mName;
		}
	}
}