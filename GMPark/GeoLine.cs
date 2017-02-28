using System;
using Xamarin.Forms.GoogleMaps;

namespace GMPark
{
	public class GeoLine
	{
		enum orientation { Bottom, Top, Left, Right };

		private orientation boundary;
		private double m, b;

		public GeoLine()
		{
			// Default case is bottom
			boundary = orientation.Bottom;
		}

		public GeoLine(double slope, double intercept)
		{
			m = slope;
			b = intercept;
			boundary = orientation.Bottom;
		}

		public void SetBottom(bool tf)
		{
			boundary = orientation.Bottom;
		}

		public void SetLeft(bool tf)
		{
			boundary = orientation.Left;
		}

		public void SetRight(bool tf)
		{
			boundary = orientation.Right;
		}

		public void SetTop(bool tf)
		{
			boundary = orientation.Top;
		}

		public bool InBoundary(Position pos)
		{
			switch (boundary)
			{
				case orientation.Bottom:
					break;

				case orientation.Left:
					break;

				case orientation.Right:
					break;

				case orientation.Top:
					break;

				default:
					break;
			}
					
			return true;
		}
	}
}
