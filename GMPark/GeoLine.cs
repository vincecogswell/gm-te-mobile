using System;
using Plugin.Geolocator.Abstractions;

namespace GMPark
{
	public class GeoLine
	{
		private double startLong, endLong;
		private double m, b;

		public GeoLine()
		{
			
		}

		public GeoLine(Location pos1, Location pos2)
		{
			if (pos1.Long > pos2.Long)
			{
				startLong = pos2.Long;
				endLong = pos1.Long;
				m = (pos1.Lat - pos2.Lat) / (pos1.Long - pos2.Long);
			}

			else
			{
				startLong = pos1.Long;
				endLong = pos2.Long;
				m = (pos2.Lat - pos1.Lat) / (pos2.Long - pos1.Long);
			}

			b = pos1.Lat - (m * pos1.Long);

		}

		public bool InBounds(Position pos)
		{
			if ((pos.Longitude > startLong) && (pos.Longitude < endLong))
			{
				return true;
			}

			else
			{
				return false;
			}
		}

		public double GetLat(double lon)
		{
			return (m * lon + b);
		}
	}
}
