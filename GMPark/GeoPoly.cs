using System;
using System.Collections.Generic;
using Plugin.Geolocator.Abstractions;

namespace GMPark
{
	public class GeoPoly
	{
		List<GeoLine> boundaries;

		public GeoPoly()
		{
			boundaries = new List<GeoLine>();
		}

		public void AddGeoLine(Location loc1, Location loc2)
		{
			boundaries.Add(new GeoLine(loc1, loc2));
		}

		public bool InFence(Position pos)
		{
			var lats = new List<double>();

			foreach (GeoLine line in boundaries)
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
			
	}
}