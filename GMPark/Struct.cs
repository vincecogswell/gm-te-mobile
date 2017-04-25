/* Struct class
 * 
 */
using System;
using Xamarin.Forms.GoogleMaps;
using System.Collections.Generic;

namespace GMPark
{
	public class Struct
	{
		private bool mActive = true;
		private string mName;
		private string mId;
		public List<Location> Entrances = new List<Location>();

		public void SetActive(bool active)
		{
			mActive = active;
		}

		public bool IsActive()
		{
			return mActive;
		}

		public void SetName(string name)
		{
			mName = name;
		}

		public string GetName()
		{
			return mName;
		}

		public string GetId()
		{
			return mId;
		}
			
		public void SetId(string id)
		{
			mId = id;
		}

		public void AddEntrance(Location loc)
		{
			Entrances.Add(loc);
		}

		public Location GetEntrance(int index)
		{
			if (Entrances.Count > index)
			{
				return Entrances[index];
			}

			else
			{
				return null;
			}
		}

		public List<Position> GetEntrances()
		{
			var ls = new List<Position>();
			foreach(Location loc in Entrances)
			{
				ls.Add(new Position(loc.Lat, loc.Long));
			}

			return ls;
		}

		public int GetEntranceCount()
		{
			return Entrances.Count;
		}
	}
}
