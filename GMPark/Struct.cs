using System;
using System.Collections.Generic;

namespace GMPark
{
	public class Struct
	{
		private bool mActive = true;
		private string mName;
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

		public void AddEntrance(Location loc)
		{
			Entrances.Add(loc);
		}
	}
}
