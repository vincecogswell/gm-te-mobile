/* Author : Rob
 * GeoLine class
 * GeoLine is the element of GeoPoly
 */
using System;
using Plugin.Geolocator.Abstractions;

namespace GMPark
{
	/* This class creates a line based on two points, then can return the points in the line
	 */
	public class GeoLine
	{
		private double startLong, endLong;
		private double m, b;

		/* Constructor
		 */
		public GeoLine()
		{
			
		}

		/* Constructor	
		 * Parameters: Location (line start), Location (line end)
		 */
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

		/* Checks to see if position is within the bounds of the line
		 * Parameters: Position (the position to be checked)
		 * Returns: bool (true if in line, false if not)
		 */
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

		/* Gets the latitude of the line at a certain longitude
		 * Parameters: double (longitude of the point)
		 * Returns: double (latitude of the line)
		 */
		public double GetLat(double lon)
		{
			return (m * lon + b);
		}
	}
}
